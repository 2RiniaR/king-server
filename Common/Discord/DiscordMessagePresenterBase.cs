using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common;

/// <summary>
/// discordのメッセージをトリガーとするイベント
/// </summary>
public abstract class DiscordMessagePresenterBase : PresenterBase
{
    /// <summary>
    /// トリガーとなったメッセージ
    /// </summary>
    public SocketUserMessage Message { get; init; } = null!;

    protected override async Task SendAppError(AppException e)
    {
        await Message.ReplyAsync(e.Message);
    }
}
