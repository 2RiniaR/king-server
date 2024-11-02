namespace Approvers.King.Common;

public abstract class MasterRecord<TKey> where TKey : notnull
{
    public abstract TKey Key { get; }
}
