using System.Text.Json;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Loxy;

public class LoxyTranslatePresenter : DiscordMessagePresenterBase
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private const int MaxInputLength = 500;
    private static readonly TimeSpan CoolTime = TimeSpan.FromSeconds(3);

    private static readonly System.Text.RegularExpressions.Regex JapaneseCharPattern =
        new(@"[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FAF\u3400-\u4DBF]", System.Text.RegularExpressions.RegexOptions.Compiled);

    protected override async Task MainAsync()
    {
        // クールダウン中なら発動しない
        var now = TimeManager.GetNow();
        var lastSend = AppCache.Instance.LoxyLastTranslateTime;
        if (lastSend.HasValue && now - lastSend < CoolTime)
        {
            return;
        }

        AppCache.Instance.LoxyLastTranslateTime = now;

        string? translatedContent;
        try
        {
            var content = Message.Content;
            if (content.Length > MaxInputLength)
            {
                content = content[..MaxInputLength];
            }

            translatedContent = await TranslateAsync(content);
        }
        catch
        {
            translatedContent = null;
        }

        if (string.IsNullOrEmpty(translatedContent))
        {
            await Message.ReplyAsync(MasterManager.LoxySettingMaster.TranslateFailedMessage);
            return;
        }

        await Message.ReplyAsync($"**{translatedContent}**");
    }

    private async Task<string?> TranslateAsync(string content)
    {
        // 入力のサニタイゼーション
        var sanitizedContent = content.Replace("\n", " ").Trim();
        var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(sanitizedContent)}&langpair=en|ja";

        try
        {
            using var response = await HttpClient.GetAsync(url);

            // ステータスコードの確認
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(responseContent);

            // 翻訳結果の取得
            if (json.RootElement.TryGetProperty("responseData", out var responseData) &&
                responseData.TryGetProperty("translatedText", out var translatedText))
            {
                var result = translatedText.GetString();
                if (!string.IsNullOrEmpty(result) && IsJapanese(result))
                {
                    return result;
                }
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    private static bool IsJapanese(string content)
    {
        return JapaneseCharPattern.IsMatch(content);
    }
}
