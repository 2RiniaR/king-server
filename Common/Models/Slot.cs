using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Approvers.King.Common;

public class Slot
{
    private const int ReelCount = 3;

    [Key] public Guid Id { get; set; }

    public int ConditionPermillage { get; set; }

    /// <summary>
    /// 調子(千分率)
    /// </summary>
    [NotMapped]
    public Multiplier Condition
    {
        get => Multiplier.FromPermillage(ConditionPermillage);
        set => ConditionPermillage = value.Permillage;
    }

    public int ExecutePrice => MasterManager.IssoSettingMaster.PricePerSlotOnce;

    /// <summary>
    /// 調子を再抽選する
    /// </summary>
    public void ShuffleCondition()
    {
        var max = MasterManager.IssoSettingMaster.SlotMaxConditionOffset;
        var min = MasterManager.IssoSettingMaster.SlotMinConditionOffset;
        Condition = RandomManager.GetRandomMultiplier(min, max);
    }

    /// <summary>
    /// スロットを実行する
    /// </summary>
    public SlotExecuteResult Execute()
    {
        var items = MasterManager.SlotItemMaster.GetAll().ToList();
        var itemCount = items.Count;

        var reelItems = new SlotItem[ReelCount];
        for (var i = 0; i < ReelCount; i++)
        {
            if (i == 0)
            {
                reelItems[i] = items[RandomManager.GetRandomInt(itemCount)];
                continue;
            }

            // 一定確率で直前と同じ出目が出る
            // 確率はマスタデータの設定値に加え、調子により変動する
            var repeatProbability = (reelItems[i - 1].RepeatProbability + Condition).Clamp(Multiplier.Zero, MasterManager.IssoSettingMaster.SlotRepeatUpperBound);
            var isRepeat = RandomManager.IsHit(repeatProbability);
            if (isRepeat)
            {
                reelItems[i] = reelItems[i - 1];
                continue;
            }

            reelItems[i] = items[RandomManager.GetRandomInt(itemCount)];
        }

        var isWin = reelItems.Select(x => x.Id).Distinct().Count() == 1;
        var resultRate = isWin ? reelItems[0].ReturnRate : Multiplier.Zero;

        return new SlotExecuteResult()
        {
            ReelItems = reelItems,
            IsWin = isWin,
            ResultRate = resultRate
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
    /// キャッシュバック倍率
    /// </summary>
    public Multiplier ResultRate { get; init; }
}
