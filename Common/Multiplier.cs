using System.Diagnostics.CodeAnalysis;

namespace Approvers.King.Common;

/// <summary>
/// 確率・倍率を表す構造体
/// </summary>
public readonly struct Multiplier : IEquatable<Multiplier>, IComparable<Multiplier>, IComparable
{
    /// <summary>
    /// 千分率 (50% -> 500)
    /// </summary>
    public int Permillage { get; }

    /// <summary>
    /// 倍率 (50% -> 0.5)
    /// </summary>
    public float Rate => Permillage / 1000f;

    /// <summary>
    /// パーセント (50% -> 50)
    /// </summary>
    public float Percent => Permillage / 10f;

    public static Multiplier Zero { get; } = new(0);
    public static Multiplier One { get; } = new(1000);
    public static Multiplier FromRate(float rate) => new((int)(rate * 1000f));
    public static Multiplier FromPercent(float percent) => new((int)(percent * 10f));
    public static Multiplier FromPermillage(int permillage) => new(permillage);

    private Multiplier(int permillage)
    {
        Permillage = permillage;
    }

    public int CompareTo(Multiplier other)
    {
        return Permillage.CompareTo(other.Permillage);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return obj is Multiplier other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Multiplier)}");
    }

    public static bool operator <(Multiplier left, Multiplier right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(Multiplier left, Multiplier right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(Multiplier left, Multiplier right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(Multiplier left, Multiplier right)
    {
        return left.CompareTo(right) >= 0;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Multiplier other)
    {
        return Permillage == other.Permillage;
    }

    public static bool operator ==(Multiplier left, Multiplier right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Multiplier left, Multiplier right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return Permillage;
    }

    public override string ToString()
    {
        return $"{Rate:P0}";
    }

    public static Multiplier operator +(Multiplier left, Multiplier right)
    {
        return new Multiplier(left.Permillage + right.Permillage);
    }

    public static Multiplier operator -(Multiplier left, Multiplier right)
    {
        return new Multiplier(left.Permillage - right.Permillage);
    }

    public static Multiplier operator *(Multiplier left, Multiplier right)
    {
        return new Multiplier((int)(left.Rate * right.Rate * 1000f));
    }

    public static Multiplier operator /(Multiplier left, Multiplier right)
    {
        return new Multiplier((int)(left.Rate / right.Rate * 1000f));
    }

    public static Multiplier operator *(Multiplier left, int right)
    {
        return new Multiplier(left.Permillage * right);
    }

    public static Multiplier operator /(Multiplier left, int right)
    {
        return new Multiplier(left.Permillage / right);
    }

    public static float operator *(float left, Multiplier right)
    {
        return left * right.Rate;
    }

    public static float operator /(float left, Multiplier right)
    {
        return left / right.Rate;
    }

    public Multiplier Clamp(Multiplier min, Multiplier max)
    {
        return new Multiplier(Math.Clamp(Permillage, min.Permillage, max.Permillage));
    }
}
