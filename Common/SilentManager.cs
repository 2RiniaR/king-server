namespace Approvers.King.Common;

public static class SilentManager
{
    private static readonly Dictionary<ulong, DateTime> SilentUsers = new();

    /// <summary>
    /// 一定時間黙らせる
    /// </summary>
    public static void SetSilent(ulong userId, TimeSpan timeSpan)
    {
        var expireTime = TimeManager.GetNow() + timeSpan;
        SilentUsers[userId] = expireTime;
    }

    /// <summary>
    /// 黙っている最中か
    /// </summary>
    public static bool IsSilent(ulong userId)
    {
        return SilentUsers.TryGetValue(userId, out var expireTime) && !TimeManager.IsExpired(expireTime);
    }
}
