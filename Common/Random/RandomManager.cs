namespace Approvers.King.Common;

public class RandomManager : Singleton<RandomManager>
{
    private readonly Random _random = new((int)DateTime.Now.Ticks & 0x0000FFFF);

    public static float GetRandomFloat(float maxExclusive)
    {
        return GetRandomFloat(0f, maxExclusive);
    }

    public static float GetRandomFloat(float minInclusive, float maxExclusive)
    {
        return (float)(minInclusive + (maxExclusive - minInclusive) * Instance._random.NextDouble());
    }

    public static int GetRandomInt(int maxExclusive)
    {
        return GetRandomInt(0, maxExclusive);
    }

    public static int GetRandomInt(int minInclusive, int maxExclusive)
    {
        return Instance._random.Next(minInclusive, maxExclusive);
    }

    public static Multiplier GetRandomMultiplier(Multiplier maxInclusive)
    {
        return GetRandomMultiplier(Multiplier.Zero, maxInclusive);
    }

    public static Multiplier GetRandomMultiplier(Multiplier minInclusive, Multiplier maxInclusive)
    {
        return Multiplier.FromPermillage(Instance._random.Next(minInclusive.Permillage, maxInclusive.Permillage + 1));
    }

    public static bool IsHit(Multiplier probability)
    {
        return GetRandomMultiplier(Multiplier.One) <= probability;
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
