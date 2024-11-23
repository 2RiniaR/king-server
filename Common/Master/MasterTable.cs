namespace Approvers.King.Common;

public abstract class MasterTable<TKey, TRecord> where TRecord : MasterRecord<TKey> where TKey : notnull
{
    private readonly Dictionary<TKey, TRecord> _records = [];

    public TRecord? Find(TKey key)
    {
        return _records.GetValueOrDefault(key);
    }

    public TRecord? FirstOrDefault(Predicate<TRecord> predicate)
    {
        return _records.Values.FirstOrDefault(x => predicate(x));
    }

    public IEnumerable<TRecord> GetAll(Predicate<TRecord>? predicate = null)
    {
        return _records.Values.Where(x => predicate == null || predicate(x));
    }

    public void Set(IEnumerable<TRecord> records)
    {
        _records.Clear();
        foreach (var record in records)
        {
            _records.Add(record.Key, record);
        }
    }
}
