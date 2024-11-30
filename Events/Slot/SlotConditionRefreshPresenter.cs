using Approvers.King.Common;

namespace Approvers.King.Events;

/// <summary>
/// スロットの調子を再抽選するイベント
/// </summary>
public class SlotConditionRefreshPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        SlotManager.Instance.ShuffleCondition();
        await SlotManager.Instance.SaveAsync();
    }
}
