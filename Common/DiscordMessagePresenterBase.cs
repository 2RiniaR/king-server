using Discord.WebSocket;

namespace Approvers.King.Common;

public abstract class DiscordMessagePresenterBase : PresenterBase
{
    public SocketUserMessage Message { get; init; } = null!;
}
