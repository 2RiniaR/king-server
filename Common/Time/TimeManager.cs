namespace Approvers.King.Common;

public class TimeManager : Singleton<TimeManager>
{
    public static TimeSpan DailyResetTime => TimeSpan.FromMilliseconds(MasterManager.IssoSettingMaster.DailyResetTime);
    public static int MonthlyResetDay => MasterManager.IssoSettingMaster.MonthlyResetDay;

    private DateTime? _debugBaseTime;
    private TimeSpan _debugTimeOffset;

    public void Initialize()
    {
        var debugDateTime = EnvironmentManager.DebugDateTime;
        if (string.IsNullOrEmpty(debugDateTime) == false)
        {
            _debugBaseTime = DateTime.Parse(debugDateTime);
        }

        _debugTimeOffset = _debugBaseTime.HasValue ? _debugBaseTime.Value - DateTime.Now.ToLocalTime() : TimeSpan.Zero;
    }

    /// <summary>
    /// アプリ内の現在時刻を取得する
    /// デバッグ機能により操作されることがある
    /// </summary>
    public static DateTime GetNow()
    {
        return DateTime.Now.ToLocalTime() + Instance._debugTimeOffset;
    }
}
