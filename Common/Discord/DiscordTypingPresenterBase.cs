using Discord.WebSocket;

namespace Approvers.King.Common;

/// <summary>
/// discordの入力中をトリガーとするイベント
/// </summary>
public abstract class DiscordTypingPresenterBase : PresenterBase
{
    public SocketUser User { get; init; } = null!;
    public ISocketMessageChannel Channel { get; init; } = null!;
}
