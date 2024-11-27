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

        var reelItems = new SlotItem[ReelCount];
        for (var i = 0; i < ReelCount; i++)
        {
            if (i == 0)
            {
                reelItems[i] = _items[RandomManager.GetRandomInt(itemCount)];
                continue;
            }

            // 一定確率で直前と同じ出目が出る
            var repeatProbability = NumberUtility.GetProbabilityFromPermillage(reelItems[i - 1].RepeatPermillage);
            var isRepeat = RandomManager.IsHit(repeatProbability);
            if (isRepeat)
            {
                reelItems[i] = reelItems[i - 1];
                continue;
            }

            reelItems[i] = _items[RandomManager.GetRandomInt(itemCount)];
        }

        var isWin = reelItems.Select(x => x.Id).Distinct().Count() == 1;
        var resultRatePermillage = isWin ? reelItems[0].ReturnRatePermillage : 0;

        return new SlotExecuteResult()
        {
            ReelItems = reelItems,
            IsWin = isWin,
            ResultRatePermillage = resultRatePermillage
        };
    }
}

public class SlotExecuteResult
{
    public required SlotItem[] ReelItems { get; init; }
    public bool IsWin { get; init; }
    public int ResultRatePermillage { get; init; }
}