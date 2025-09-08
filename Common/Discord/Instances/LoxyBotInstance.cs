using System.Text.RegularExpressions;
using Approvers.King.Events.Loxy;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common.Instances;

public class LoxyBotInstance : DiscordBotInstanceBase
{
    public LoxyBotInstance() : base(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.MessageContent |
                         GatewayIntents.Guilds |
                         GatewayIntents.GuildMessages
    })
    {
    }

    private static readonly Regex UrlPattern = new(@"https?://|www\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex DiscordMentionPattern = new(@"<[@#:].+?>", RegexOptions.Compiled);
    private static readonly Regex JapanesePattern = new(@"[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FAF]", RegexOptions.Compiled);
    private static readonly Regex NumberOnlyPattern = new(@"^\d+$", RegexOptions.Compiled);
    private static readonly Regex EnglishLettersPattern = new(@"[a-zA-Z]{2,}", RegexOptions.Compiled);
    private static readonly Regex AllowedCharsPattern = new(@"^[a-zA-Z0-9\s\.\,\!\?\'\-\(\)\[\]\{\}\:\;""]+$", RegexOptions.Compiled);
    private static readonly Regex EnglishWordPattern = new(@"\b[a-zA-Z]{2,}\b", RegexOptions.Compiled);
    private static readonly Regex CodePatternSyntax = new(@"[{}\[\];=<>_\+\-/\|:]", RegexOptions.Compiled);

    public override string DisplayName => "Loxy";

    protected override string GetToken()
    {
        return EnvironmentManager.DiscordSecretLoxy;
    }

    public void RegisterEvents()
    {
        Client.MessageReceived += message =>
        {
            OnMessageReceived(message);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// 英文かどうか
    /// </summary>
    private static bool IsEnglish(string content)
    {
        // 空文字列や短すぎるメッセージは除外
        if (string.IsNullOrWhiteSpace(content) || content.Length < 3)
            return false;

        // URLパターンを除外（http://, https://, www.）
        if (UrlPattern.IsMatch(content))
            return false;

        // Discordメンション、チャンネル参照、絵文字を除外
        if (DiscordMentionPattern.IsMatch(content))
            return false;

        // 日本語文字（ひらがな、カタカナ、漢字）が含まれている場合は除外
        if (JapanesePattern.IsMatch(content))
            return false;

        // 数字のみの文字列は除外
        if (NumberOnlyPattern.IsMatch(content))
            return false;

        // プログラミング言語の構文記号が含まれている（括弧、セミコロン、演算子など）
        if (CodePatternSyntax.IsMatch(content))
            return false;

        // 2文字以上の連続した英字が含まれているかチェック
        if (!EnglishLettersPattern.IsMatch(content))
            return false;

        // 英字・数字・基本的な記号・スペースのみで構成されているかチェック
        // （句読点、感嘆符、疑問符、カンマ、ピリオド、アポストロフィ、ハイフン等を許可）
        if (!AllowedCharsPattern.IsMatch(content))
            return false;

        // マスタの除外リストに含まれている場合は除外
        if (MasterManager.LoxyIgnoreEnglishMaster.GetAll().Any(x => content.Contains(x.Value)))
            return false;

        // 最低1つの英単語（2文字以上の英字の連続）が含まれていることを再確認
        return EnglishWordPattern.IsMatch(content);
    }

    private void OnMessageReceived(SocketMessage message)
    {
        // botは弾く
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        if (IsEnglish(message.Content))
        {
            ExecuteMessageEventAsync<LoxyTranslatePresenter>(userMessage).Run();
        }
    }
}
