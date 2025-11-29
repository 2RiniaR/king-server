using Approvers.King.Common;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Events.Isso;

public class VoiceNotificationPresenter : PresenterBase
{
    public SocketUser User { get; init; } = null!;
    public SocketVoiceState Before { get; init; }
    public SocketVoiceState After { get; init; }

    protected override async Task MainAsync()
    {
        // Botは対象外
        if (User.IsBot) return;

        // SocketGuildUserにキャストできない場合は処理終了
        if (User is not SocketGuildUser guildUser) return;

        var beforeChannel = Before.VoiceChannel;
        var afterChannel = After.VoiceChannel;

        // 同一チャンネル内の状態変更（ミュート等）は無視
        if (beforeChannel?.Id == afterChannel?.Id) return;

        if (beforeChannel == null && afterChannel != null)
        {
            // 入室
            await HandleJoinAsync(guildUser, afterChannel);
        }
        else if (beforeChannel != null && afterChannel == null)
        {
            // 退出
            await HandleLeaveAsync(guildUser, beforeChannel);
        }
        else if (beforeChannel != null && afterChannel != null)
        {
            // 移動
            await HandleMoveAsync(guildUser, afterChannel);
        }
    }

    private async Task HandleJoinAsync(SocketGuildUser guildUser, SocketVoiceChannel channel)
    {
        var message = $"{guildUser.DisplayName}が{channel.Name}に入りました";
        var randomMessage = GetRandomMessage(RandomMessageType.VoiceDiffJoin);
        var embed = CreateNotificationEmbed(guildUser, message, randomMessage, Color.Green);

        await SendNotificationAsync(embed);
    }

    private async Task HandleLeaveAsync(SocketGuildUser guildUser, SocketVoiceChannel channel)
    {
        var message = $"{guildUser.DisplayName}が{channel.Name}から抜けました";
        var randomMessage = GetRandomMessage(RandomMessageType.VoiceDiffLeave);
        var embed = CreateNotificationEmbed(guildUser, message, randomMessage, Color.Red);

        await SendNotificationAsync(embed);
    }

    private async Task HandleMoveAsync(SocketGuildUser guildUser, SocketVoiceChannel toChannel)
    {
        var message = $"{guildUser.DisplayName}が{toChannel.Name}に入りました";
        var randomMessage = GetRandomMessage(RandomMessageType.VoiceDiffJoin);
        var embed = CreateNotificationEmbed(guildUser, message, randomMessage, Color.Green);

        await SendNotificationAsync(embed);
    }

    private Embed CreateNotificationEmbed(
        SocketGuildUser guildUser,
        string mainMessage,
        string? randomMessage,
        Color color)
    {
        var avatarUrl = guildUser.GetGuildAvatarUrl() ?? guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor("創造主からのお知らせ")
            .WithColor(color)
            .WithTitle(mainMessage)
            .WithDescription(randomMessage)
            .WithThumbnailUrl(avatarUrl);

        return embedBuilder.Build();
    }

    private string? GetRandomMessage(RandomMessageType type)
    {
        var messages = MasterManager.IssoRandomMessageMaster.GetAll(x => x.Type == type).ToList();
        if (messages.Count == 0) return null;

        return RandomManager.PickRandom(messages).Content;
    }

    private async Task SendNotificationAsync(Embed embed)
    {
        var channel = DiscordManager.IssoBot.GetMainChannel();
        if (channel == null) return;

        await channel.SendMessageAsync(embed: embed);
    }
}
