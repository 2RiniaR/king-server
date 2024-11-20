namespace Approvers.King.Common;

public static class NumberUtility
{
    public static DateTime GetBaseDateTime()
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public static float GetSecondsFromMilliseconds(float milliseconds)
    {
        return milliseconds / 1000f;
    }

    public static TimeSpan GetTimeSpanFromMilliseconds(float milliseconds)
    {
        return TimeSpan.FromMilliseconds(milliseconds);
    }

    public static float GetPercentFromPermillage(int permillage)
    {
        return permillage / 1000f;
    }

    public static int GetPermillageFromPercent(float percent)
    {
        return (int)(percent * 1000f);
    }
}
