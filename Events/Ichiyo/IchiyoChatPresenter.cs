using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Ichiyo;

public class IchiyoChatPresenter : DiscordMessagePresenterBase
{
    private const int MaxInputLength = 2000;
    private static readonly TimeSpan ProcessTimeout = TimeSpan.FromMinutes(2);

    // 危険なプロンプトインジェクションパターン
    private static readonly Regex[] DangerousPatterns =
    [
        new Regex(@"ignore\s+(all\s+)?(previous|above|prior)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"disregard\s+(all\s+)?(previous|above|prior)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"forget\s+(all\s+)?(previous|above|prior)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"new\s+instructions?:", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"system\s*prompt:", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"you\s+are\s+now\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"pretend\s+(to\s+be|you\s+are)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"act\s+as\s+(if\s+you|a\s+different)", RegexOptions.IgnoreCase | RegexOptions.Compiled)
    ];

    // 出力に含まれてはいけないキーワード（システムプロンプト漏洩検出用）
    private static readonly string[] SensitiveKeywords =
    [
        "system prompt",
        "my instructions",
        "original instructions",
        "initial prompt"
    ];

    /// <summary>
    /// セッションを継続する場合のセッションID（nullの場合は新規セッション）
    /// </summary>
    public string? ResumeSessionId { get; set; }

    protected override async Task MainAsync()
    {
        var settings = MasterManager.IchiyoSettingMaster;

        // 入力テキストの取得とサニタイズ
        var userInput = SanitizeInput(Message.Content);

        if (string.IsNullOrWhiteSpace(userInput))
        {
            await Message.ReplyAsync(settings.EmptyInputMessage);
            return;
        }

        // ユーザー名を取得
        var userName = GetUserName(Message.Author);

        // 現在時刻を取得（JST）
        var now = TimeManager.GetNow();
        var timeStr = now.ToString("yyyy/MM/dd HH:mm");

        // 入力をJSON形式で構造化（AIがフォーマットを模倣しないように）
        var inputData = new { time = $"{timeStr} JST", user = userName, message = userInput };
        var formattedInput = JsonSerializer.Serialize(inputData);

        // タイピング表示を開始
        using var typingState = Message.Channel.EnterTypingState();

        try
        {
            var result = await ExecuteClaudeCliAsync(formattedInput, settings.SystemPrompt, ResumeSessionId);

            if (result == null)
            {
                await Message.ReplyAsync(settings.ErrorMessage);
                return;
            }

            if (result.IsContextLimit)
            {
                // context-window limitに達した
                await Message.ReplyAsync(settings.ContextLimitMessage);

                // セッションを無効化（該当するセッションを特定する必要があるが、
                // リプライ元のメッセージIDで無効化する）
                if (Message.Reference?.MessageId.IsSpecified == true)
                {
                    IchiyoSessionStore.Instance.InvalidateSession(Message.Reference.MessageId.Value);
                }

                return;
            }

            if (string.IsNullOrWhiteSpace(result.Response))
            {
                await Message.ReplyAsync(settings.ErrorMessage);
                return;
            }

            // 出力の検証（機密情報漏洩チェック）
            var sanitizedResponse = ValidateAndSanitizeOutput(result.Response);

            // リプライを送信
            var replyMessage = await Message.ReplyAsync(sanitizedResponse);

            // セッションを登録
            if (!string.IsNullOrEmpty(result.SessionId))
            {
                IchiyoSessionStore.Instance.RegisterSession(replyMessage.Id, result.SessionId);
            }
        }
        catch (TimeoutException)
        {
            await Message.ReplyAsync(settings.TimeoutMessage);
        }
        catch (Exception ex)
        {
            LogManager.LogError($"[Ichiyo] Error executing claude-cli: {ex.Message}");
            await Message.ReplyAsync(settings.ErrorMessage);
        }
    }

    /// <summary>
    /// 入力テキストをサニタイズする
    /// </summary>
    private string SanitizeInput(string content)
    {
        // メンションを除去
        var sanitized = Regex.Replace(content, @"<@!?\d+>", "").Trim();

        // 制御文字を除去
        sanitized = Regex.Replace(sanitized, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

        // 長さ制限
        if (sanitized.Length > MaxInputLength)
        {
            sanitized = sanitized[..MaxInputLength];
        }

        // 危険なパターンを無害化
        foreach (var pattern in DangerousPatterns)
        {
            sanitized = pattern.Replace(sanitized, "[filtered]");
        }

        return sanitized;
    }

    /// <summary>
    /// 出力を検証し、必要に応じてサニタイズする
    /// </summary>
    private static string ValidateAndSanitizeOutput(string response)
    {
        var lower = response.ToLowerInvariant();

        foreach (var keyword in SensitiveKeywords)
        {
            if (lower.Contains(keyword))
            {
                LogManager.LogError($"[Ichiyo] Detected sensitive keyword in output: {keyword}");
                return "[出力がフィルタされました]";
            }
        }

        // Discordメッセージの長さ制限
        if (response.Length > 2000)
        {
            response = response[..1997] + "...";
        }

        return response;
    }

    /// <summary>
    /// claude-cliを実行してレスポンスを取得する
    /// </summary>
    private async Task<ClaudeCliResult?> ExecuteClaudeCliAsync(string userInput, string systemPrompt, string? resumeSessionId)
    {
        // コマンド引数を構築
        var arguments = new List<string>
        {
            "-p",
            "--output-format", "json",
            "--model", "claude-3-haiku-20240307",
            "--tools", ""
        };

        if (!string.IsNullOrEmpty(resumeSessionId))
        {
            arguments.Add("--resume");
            arguments.Add(resumeSessionId);
        }
        else if (!string.IsNullOrEmpty(systemPrompt))
        {
            arguments.Add("--system-prompt");
            arguments.Add(systemPrompt);
        }

        arguments.Add(userInput);

        var startInfo = new ProcessStartInfo
        {
            FileName = "claude",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var arg in arguments)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = new Process { StartInfo = startInfo };
        var outputBuilder = new System.Text.StringBuilder();
        var errorBuilder = new System.Text.StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                outputBuilder.AppendLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                errorBuilder.AppendLine(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var cts = new CancellationTokenSource(ProcessTimeout);

        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
                // ignore
            }

            throw new TimeoutException("claude-cli execution timed out");
        }

        var output = outputBuilder.ToString().Trim();
        var errorOutput = errorBuilder.ToString().Trim();

        if (!string.IsNullOrEmpty(errorOutput))
        {
            LogManager.LogError($"[Ichiyo] claude-cli stderr: {errorOutput}");
        }

        if (string.IsNullOrEmpty(output))
        {
            LogManager.LogError("[Ichiyo] claude-cli returned empty output");
            return null;
        }

        return ParseClaudeCliOutput(output);
    }

    /// <summary>
    /// claude-cliの出力をパースする
    /// </summary>
    private static ClaudeCliResult? ParseClaudeCliOutput(string output)
    {
        try
        {
            // claude-cliは複数行のJSONを出力する可能性があるので、最後の有効なJSONオブジェクトを探す
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string? sessionId = null;
            string? response = null;
            bool isContextLimit = false;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (!trimmedLine.StartsWith('{'))
                    continue;

                try
                {
                    var json = JsonDocument.Parse(trimmedLine);
                    var root = json.RootElement;

                    // session_idを探す
                    if (root.TryGetProperty("session_id", out var sessionIdElement))
                    {
                        sessionId = sessionIdElement.GetString();
                    }

                    // context limitのチェック
                    if (root.TryGetProperty("is_error", out var isError) && isError.GetBoolean())
                    {
                        if (root.TryGetProperty("error", out var error))
                        {
                            var errorStr = error.GetString() ?? "";
                            if (errorStr.Contains("context") && errorStr.Contains("limit"))
                            {
                                isContextLimit = true;
                            }
                        }
                    }

                    // resultを探す（文字列として）
                    if (root.TryGetProperty("result", out var resultElement) &&
                        resultElement.ValueKind == JsonValueKind.String)
                    {
                        response = resultElement.GetString();
                    }
                }
                catch (JsonException)
                {
                    // このラインはパースできなかった、次へ
                    continue;
                }
            }

            if (isContextLimit)
            {
                return new ClaudeCliResult
                {
                    SessionId = sessionId,
                    Response = null,
                    IsContextLimit = true
                };
            }

            if (string.IsNullOrEmpty(response))
            {
                LogManager.LogError($"[Ichiyo] Could not find result in output: {output}");
                return null;
            }

            return new ClaudeCliResult
            {
                SessionId = sessionId,
                Response = response,
                IsContextLimit = false
            };
        }
        catch (Exception ex)
        {
            LogManager.LogError($"[Ichiyo] Failed to parse claude-cli output: {ex.Message}, output: {output}");
            return null;
        }
    }

    private class ClaudeCliResult
    {
        public string? SessionId { get; init; }
        public string? Response { get; init; }
        public bool IsContextLimit { get; init; }
    }

    /// <summary>
    /// ユーザー名を取得する
    /// マスタに登録があればその名前を使用し、なければDiscordの表示名を使用
    /// </summary>
    private static string GetUserName(IUser author)
    {
        var userId = author.Id.ToString();

        // マスタから検索
        var ichiyoUser = MasterManager.IchiyoUserMaster.Find(userId);
        if (ichiyoUser != null)
        {
            return ichiyoUser.Name;
        }

        // マスタになければDiscordの表示名を使用
        if (author is IGuildUser guildUser)
        {
            return guildUser.DisplayName;
        }

        return author.Username;
    }
}
