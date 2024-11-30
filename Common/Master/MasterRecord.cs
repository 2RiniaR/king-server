namespace Approvers.King.Common;

/// <summary>
/// マスタデータの行
/// </summary>
public abstract class MasterRecord<TKey> where TKey : notnull
{
    public abstract TKey Key { get; }
}
