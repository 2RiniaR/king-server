namespace Approvers.King.Common;

public static class EnumrableExtensions
{
    public static TDist? FirstTypeOrDefault<T, TDist>(this IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            if (item is TDist dist)
            {
                return dist;
            }
        }

        return default;
    }
}
