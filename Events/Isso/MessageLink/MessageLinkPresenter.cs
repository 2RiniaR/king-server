using System.Text.RegularExpressions;
using Approvers.King.Common;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Events.Isso;

public class MessageLinkPresenter : DiscordMessagePresenterBase
{
    // Discordメッセージリンクの正規表現パターン
    // https://discord.com/channels/{guildId}/{channelId}/{messageId}
    private static readonly Regex MessageLinkRegex = new(
        @"https?://(?:www\.)?discord(?:app)?\.com/channels/(\d+)/(\d+)/(\d+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    protected override async Task MainAsync()
    {
        var matches = MessageLinkRegex.Matches(Message.Content);
        
        // メッセージリンクが含まれていない場合は何もしない
        if (matches.Count == 0)
        {
            return;
        }

        // 各メッセージリンクを処理
        foreach (Match match in matches)
        {
            await ProcessMessageLink(Message, match);
        }
    }

    private async Task ProcessMessageLink(SocketUserMessage originalMessage, Match match)
    {
        try
        {
            // リンクからID情報を抽出
            if (!ulong.TryParse(match.Groups[1].Value, out var guildId) ||
                !ulong.TryParse(match.Groups[2].Value, out var channelId) ||
                !ulong.TryParse(match.Groups[3].Value, out var messageId))
            {
                return;
            }

            // ギルドの取得
            var guild = DiscordManager.IssoBot.Client.GetGuild(guildId);
            if (guild == null)
            {
                await originalMessage.ReplyAsync(embed: ErrorEmbed("参照先のサーバーにアクセスできません。"));
                return;
            }

            // チャンネルの取得
            var channel = guild.GetTextChannel(channelId);
            if (channel == null)
            {
                await originalMessage.ReplyAsync(embed: ErrorEmbed("参照先のチャンネルにアクセスできません。"));
                return;
            }

            // メッセージの取得
            var referencedMessage = await channel.GetMessageAsync(messageId);
            if (referencedMessage == null)
            {
                await originalMessage.ReplyAsync(embed: ErrorEmbed("参照先のメッセージが見つかりません。"));
                return;
            }

            // Embedを作成
            var embed = CreateMessageEmbed(referencedMessage, channel, guild);
            
            // Replyとして送信
            await originalMessage.ReplyAsync(embed: embed);
        }
        catch (Exception ex)
        {
            LogManager.LogError($"メッセージリンク処理中にエラーが発生しました: {ex.Message}");
            await originalMessage.ReplyAsync(embed: ErrorEmbed("メッセージの取得中にエラーが発生しました。"));
        }
    }

    private Embed CreateMessageEmbed(IMessage message, ITextChannel channel, IGuild guild)
    {
        // サーバープロフィールを取得
        var guildUser = message.Author as IGuildUser;
        var displayName = guildUser?.DisplayName ?? message.Author.Username;
        var avatarUrl = guildUser?.GetGuildAvatarUrl() ?? message.Author.GetAvatarUrl() ?? message.Author.GetDefaultAvatarUrl();
        
        var embedBuilder = new EmbedBuilder()
            .WithColor(new Color(0x7289da)) // Discord風の青色
            .WithAuthor(
                name: displayName,
                iconUrl: avatarUrl
            )
            .WithDescription(string.IsNullOrWhiteSpace(message.Content) 
                ? "*（メッセージ内容なし）*" 
                : message.Content.Length > 2048 
                    ? message.Content.Substring(0, 2045) + "..." 
                    : message.Content)
            .WithTimestamp(message.Timestamp)
            .WithFooter($"#{channel.Name} • {guild.Name}");

        // 添付ファイルがある場合
        if (message.Attachments.Count > 0)
        {
            var attachment = message.Attachments.First();
            
            // 画像の場合はEmbedに表示
            if (IsImageFile(attachment.Filename))
            {
                embedBuilder.WithImageUrl(attachment.Url);
            }
            
            // その他の添付ファイルは情報として追加
            if (message.Attachments.Count == 1)
            {
                embedBuilder.AddField("添付ファイル", $"[{attachment.Filename}]({attachment.Url})", inline: true);
            }
            else
            {
                embedBuilder.AddField($"添付ファイル ({message.Attachments.Count}件)", 
                    string.Join("\n", message.Attachments.Select(a => $"[{a.Filename}]({a.Url})")), 
                    inline: false);
            }
        }

        // Embedがある場合
        if (message.Embeds.Count > 0)
        {
            embedBuilder.AddField("Embed", $"{message.Embeds.Count}件のEmbedが含まれています", inline: true);
        }

        // リアクションがある場合
        if (message.Reactions.Count > 0)
        {
            var reactionText = string.Join(" ", 
                message.Reactions.Take(10).Select(r => $"{r.Key.Name} {r.Value.ReactionCount}"));
            
            if (message.Reactions.Count > 10)
            {
                reactionText += $" 他{message.Reactions.Count - 10}件";
            }
            
            embedBuilder.AddField("リアクション", reactionText, inline: false);
        }

        // メッセージへのリンクを追加
        embedBuilder.AddField("元のメッセージ", 
            $"[メッセージにジャンプ](https://discord.com/channels/{guild.Id}/{channel.Id}/{message.Id})", 
            inline: true);

        return embedBuilder.Build();
    }

    private static bool IsImageFile(string filename)
    {
        var imageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp" };
        return imageExtensions.Any(ext => filename.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}