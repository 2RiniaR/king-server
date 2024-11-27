using Approvers.King.Common;

namespace Approvers.King.Events;

public class SlotConditionRefreshPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        SlotManager.Instance.RefreshCondition();
        await SlotManager.Instance.SaveAsync();
    }
}
