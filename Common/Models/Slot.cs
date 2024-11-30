using System.ComponentModel.DataAnnotations;

namespace Approvers.King.Common;

public class Slot
{
    private const int ReelCount = 3;

    [Key] public Guid Id { get; set; }

    /// <summary>
    /// 調子(千分率)
    /// </summary>
    public int ConditionPermillage { get; set; }

    public int ExecutePrice => MasterManager.SettingMaster.PricePerSlotOnce;

    /// <summary>
    /// 調子を再抽選する
    /// </summary>
    public void ShuffleCondition()
    {
        var max = MasterManager.SettingMaster.SlotMaxConditionOffsetPermillage;
        var min = MasterManager.SettingMaster.SlotMinConditionOffsetPermillage;
        ConditionPermillage = RandomManager.GetRandomInt(min, max + 1);
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
            var repeatPermillage = Math.Clamp(reelItems[i - 1].RepeatPermillage + ConditionPermillage, 0, MasterManager.SettingMaster.SlotRepeatPermillageUpperBound);
            var repeatProbability = NumberUtility.GetProbabilityFromPermillage(repeatPermillage);
            var isRepeat = RandomManager.IsHit(repeatProbability);
            if (isRepeat)
            {
                reelItems[i] = reelItems[i - 1];
                continue;
            }

            reelItems[i] = items[RandomManager.GetRandomInt(itemCount)];
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
