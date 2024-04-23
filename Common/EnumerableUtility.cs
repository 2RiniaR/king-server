namespace Approvers.King.Common;

public static class EnumerableUtility
{
    public static IEnumerable<T> Repeat<T>(T value, int count)
    {
        for (var i = 0; i < count; i++) yield return value;
    }
}
