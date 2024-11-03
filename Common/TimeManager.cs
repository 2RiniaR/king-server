namespace Approvers.King.Common;

public static class TimeManager
{
    public static TimeSpan DailyResetTime => TimeSpan.FromMilliseconds(MasterManager.SettingMaster.DailyResetTime);
    public static int MonthlyResetDay => MasterManager.SettingMaster.MonthlyResetDay;
    public static DateTime Birthday => new DateTime(1, MasterManager.SettingMaster.BirthdayMonth,
        MasterManager.SettingMaster.BirthdayDay);
    
    /// <summary>
    ///     アプリ内の現在時刻を取得する
    ///     デバッグ機能により操作されることがある
    /// </summary>
    public static DateTime GetNow()
    {
        return DateTime.Now.ToLocalTime();
    }

    /// <summary>
    ///     過ぎていればtrue
    /// </summary>
    public static bool IsExpired(DateTime time)
    {
        return GetNow() > time;
    }
}
