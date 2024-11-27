namespace Approvers.King.Common;

public class RandomManager : Singleton<RandomManager>
{
    private readonly Random _random = new((int)DateTime.Now.Ticks & 0x0000FFFF);

    public static float GetRandomFloat(float max)
    {
        return GetRandomFloat(0f, max);
    }

    public static float GetRandomFloat(float min, float max)
    {
        return (float)(min + (max - min) * Instance._random.NextDouble());
    }

    public static int GetRandomInt(int max)
    {
        return GetRandomInt(0, max);
    }

    public static int GetRandomInt(int min, int max)
    {
        return Instance._random.Next(min, max);
    }

    public static bool IsHit(float probability)
    {
        return GetRandomFloat(1f) <= probability;
    }

    public static T PickRandom<T>(IEnumerable<T> source)
    {
        var array = source.ToArray();
        return array[Instance._random.Next(array.Length)];
    }

    public static IEnumerable<T> Shuffle<T>(IEnumerable<T> list)
    {
        return list.OrderBy(_ => Guid.NewGuid());
    }
}
