namespace Approvers.King.Common;

public class TimeManager : Singleton<TimeManager>
{
    public static TimeSpan DailyResetTime => TimeSpan.FromMilliseconds(MasterManager.SettingMaster.DailyResetTime);
    public static int MonthlyResetDay => MasterManager.SettingMaster.MonthlyResetDay;
    public static DateTime Birthday => new(1, MasterManager.SettingMaster.BirthdayMonth, MasterManager.SettingMaster.BirthdayDay);

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

    /// <summary>
    /// 時間を過ぎていればtrue
    /// </summary>
    public static bool IsExpired(DateTime time)
    {
        return GetNow() > time;
    }
}
