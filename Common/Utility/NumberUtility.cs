namespace Approvers.King.Common;

public static class NumberUtility
{
    public static float GetSecondsFromMilliseconds(float milliseconds)
    {
        return milliseconds / 1000f;
    }
}
