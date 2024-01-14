namespace Approvers.King.Common;

public static class RandomUtility
{
    private static readonly Random Random = new();

    public static float GetRandomFloat(float max)
    {
        return GetRandomFloat(0f, max);
    }

    public static float GetRandomFloat(float min, float max)
    {
        return (float)(min + (max - min) * Random.NextDouble());
    }

    public static bool IsHit(float probability)
    {
        return GetRandomFloat(1f) <= probability;
    }
}