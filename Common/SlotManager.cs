namespace Approvers.King.Common;

public class SlotManager : Singleton<SlotManager>
{
    private readonly List<SlotItem> _items = [];
    private const int ReelCount = 3;

    public void LoadMaster()
    {
        _items.Clear();
        var items = MasterManager.SlotItemMaster.GetAll();
        _items.AddRange(items);
    }

    public SlotExecuteResult Execute()
    {
        var itemCount = _items.Count;
        var reelItems = EnumerableUtility
            .Repeat(() => _items[RandomUtility.GetRandomInt(itemCount)], ReelCount)
            .ToArray();

        var isReelSame = reelItems.Select(x => x.Id).Distinct().Count() == 1;
        var resultRatePermillage = isReelSame ? reelItems[0].ReturnRatePermillage : 0;

        return new SlotExecuteResult()
        {
            ReelItems = reelItems,
            ResultRatePermillage = resultRatePermillage
        };
    }
}

public class SlotExecuteResult
{
    public SlotItem[] ReelItems { get; set; }
    public int ResultRatePermillage { get; set; }
}
