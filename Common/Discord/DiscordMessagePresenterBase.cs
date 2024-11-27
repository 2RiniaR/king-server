using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common;

public abstract class DiscordMessagePresenterBase : PresenterBase
{
    public SocketUserMessage Message { get; init; } = null!;

    protected override async Task SendAppError(AppException e)
    {
        await Message.ReplyAsync(e.Message);
    }
}
