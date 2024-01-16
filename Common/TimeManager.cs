namespace Approvers.King.Common;

public static class TimeManager
{
    /// <summary>
    /// アプリ内の現在時刻を取得する
    /// デバッグ機能により操作されることがある
    /// </summary>
    public static DateTime GetNow()
    {
        return DateTime.Now.ToLocalTime();
    }

    /// <summary>
    /// 過ぎていればtrue
    /// </summary>
    public static bool IsExpired(DateTime time)
    {
        return GetNow() > time;
    }
}
