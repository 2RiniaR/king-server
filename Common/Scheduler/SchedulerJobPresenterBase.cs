namespace Approvers.King.Common;

public abstract class SchedulerJobPresenterBase : PresenterBase
{
    protected override async Task SendAppError(AppException e)
    {
        await DiscordManager.GetMainChannel().SendMessageAsync(e.Message);
    }
}
