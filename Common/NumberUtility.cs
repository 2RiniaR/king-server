namespace Approvers.King.Common;

public static class NumberUtility
{
    public static float GetSecondsFromMilliseconds(float milliseconds)
    {
        return milliseconds / 1000f;
    }
    
    public static TimeSpan GetTimeSpanFromMilliseconds(float milliseconds)
    {
        return TimeSpan.FromMilliseconds(milliseconds);
    }
    
    public static float GetPercentFromPermillage(float permillage)
    {
        return permillage / 1000f;
    }
}
