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

    protected override async Task MainAsync()
    {
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

            // レスポンスステータスの確認
            if (json.RootElement.TryGetProperty("responseStatus", out var responseStatus))
            {
                if (responseStatus.GetInt32() != 200)
                {
                    return null;
                }
            }

            // 翻訳結果の取得
            if (json.RootElement.TryGetProperty("responseData", out var responseData) &&
                responseData.TryGetProperty("translatedText", out var translatedText))
            {
                var translation = translatedText.GetString();

                // 翻訳結果の検証
                if (!string.IsNullOrWhiteSpace(translation) &&
                    !translation.Equals("NO QUERY SPECIFIED. EXAMPLE REQUEST: GET?Q=HELLO&LANGPAIR=EN|IT", StringComparison.OrdinalIgnoreCase))
                {
                    return translation;
                }
            }
        }
        catch
        {
            return null;
        }

        return null;
    }
}
