using Approvers.King.Common;

namespace Approvers.King.Events.Isso;

/// <summary>
/// スロットの調子を再抽選するイベント
/// </summary>
public class SlotConditionRefreshPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var slot = await app.GetDefaultSlotAsync();
        slot.ShuffleCondition();

        await app.SaveChangesAsync();
    }
}
