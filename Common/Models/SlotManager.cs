namespace Approvers.King.Common;

public class SlotManager : Singleton<SlotManager>
{
    private readonly List<SlotItem> _items = [];

    /// <summary>
    /// 調子(千分率)
    /// </summary>
    private int _conditionOffsetPermillage;

    private const int ReelCount = 3;

    /// <summary>
    /// マスタデータを読み込む
    /// </summary>
    public void LoadMaster()
    {
        _items.Clear();
        var items = MasterManager.SlotItemMaster.GetAll();
        _items.AddRange(items);
    }

    /// <summary>
    /// 現在の状態を読み込む
    /// </summary>
    public async Task LoadAsync()
    {
        await using var app = AppService.CreateSession();
        _conditionOffsetPermillage = await app.AppStates.GetIntAsync(AppStateType.SlotConditionOffsetPermillage) ?? 0;
    }

    /// <summary>
    /// 現在の状態を保存する
    /// </summary>
    public async Task SaveAsync()
    {
        await using var app = AppService.CreateSession();
        await app.AppStates.SetIntAsync(AppStateType.SlotConditionOffsetPermillage, _conditionOffsetPermillage);
        await app.SaveChangesAsync();
    }

    /// <summary>
    /// 調子を再抽選する
    /// </summary>
    public void ShuffleCondition()
    {
        var max = MasterManager.SettingMaster.SlotMaxConditionOffsetPermillage;
        var min = MasterManager.SettingMaster.SlotMinConditionOffsetPermillage;
        _conditionOffsetPermillage = RandomManager.GetRandomInt(min, max + 1);
    }

    /// <summary>
    /// スロットを実行する
    /// </summary>
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
            // 確率はマスタデータの設定値に加え、調子により変動する
            var repeatPermillage = Math.Clamp(reelItems[i - 1].RepeatPermillage + _conditionOffsetPermillage, 0, MasterManager.SettingMaster.SlotRepeatPermillageUpperBound);
            var repeatProbability = NumberUtility.GetProbabilityFromPermillage(repeatPermillage);
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
    /// <summary>
    /// 出目
    /// </summary>
    public required SlotItem[] ReelItems { get; init; }

    /// <summary>
    /// 出目が揃ったかどうか
    /// </summary>
    public bool IsWin { get; init; }

    /// <summary>
    /// キャッシュバック倍率(千分率)
    /// </summary>
    public int ResultRatePermillage { get; init; }
}
