using Approvers.King.Common;

namespace Approvers.King.Events;

public class SlotConditionRefreshPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        LogManager.Log("hoge");
        SlotManager.Instance.RefreshCondition();
        await SlotManager.Instance.SaveAsync();
    }
}
