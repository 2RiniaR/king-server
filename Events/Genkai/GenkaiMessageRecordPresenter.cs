using Approvers.King.Common;

namespace Approvers.King.Events.Genkai;

/// <summary>
/// 投稿者の最終投稿日時を記録するイベント
/// </summary>
public class GenkaiMessageRecordPresenter : DiscordMessagePresenterBase
{
    /// <summary>
    /// 投稿者のLastMessageAtをこのメッセージの日時にする
    /// </summary>
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var user = await app.FindOrCreateUserAsync(Message.Author.Id);
        user.LastMessageAt = Message.Timestamp.LocalDateTime;

        await app.SaveChangesAsync();
    }
}
