using Discord;

namespace Approvers.King.Common;

/// <summary>
/// discordのメッセージをトリガーとするイベント
/// </summary>
public abstract class DiscordMessageDeletePresenterBase : PresenterBase
{
    /// <summary>
    /// 削除されたメッセージ
    /// </summary>
    public IMessage Message { get; init; } = null!;

    /// <summary>
    /// メッセージが削除されたチャンネル
    /// </summary>
    public IMessageChannel Channel { get; init; } = null!;
}
