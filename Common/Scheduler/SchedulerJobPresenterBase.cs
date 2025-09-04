namespace Approvers.King.Common;

/// <summary>
/// 時間をトリガーとするイベント
/// </summary>
public abstract class SchedulerJobPresenterBase : PresenterBase
{
    protected override async Task SendAppError(AppException e)
    {
        await DiscordManager.IssoBot.GetMainChannel().SendMessageAsync(e.Message);
    }
}
