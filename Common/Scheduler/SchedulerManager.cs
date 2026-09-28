using Approvers.King.Events;
using Approvers.King.Events.Genkai;
using Approvers.King.Events.Isso;

namespace Approvers.King.Common;

/// <summary>
/// 時間で実行されるイベントを管理する
/// </summary>
public static class SchedulerManager
{
    private static readonly SchedulerTimer _timer = new();

    public static void Initialize()
    {
        _timer.Initialize();
    }

    public static void RegisterEvents()
    {
        _timer.RegisterDaily<DailyResetPresenter>(TimeManager.DailyResetTime);
        _timer.RegisterMonthly<MonthlyResetPresenter>(TimeManager.MonthlyResetDay, TimeManager.DailyResetTime);
        _timer.RegisterOn<SlotConditionRefreshPresenter>(x => x.Minute is 0);
        _timer.RegisterOn<GenkaiActiveRoleRefreshPresenter>(x => x.Minute % 5 is 0);
    }
}
