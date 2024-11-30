using System.ComponentModel.DataAnnotations;

namespace Approvers.King.Common;

public class User
{
    [Key] public ulong DiscordId { get; set; }
    public int MonthlyGachaPurchasePrice { get; set; }
    public int MonthlySlotProfitPrice { get; set; }
    public int TodaySlotExecuteCount { get; set; }

    public User DeepCopy()
    {
        var user = (User)MemberwiseClone();
        return user;
    }

    public void ResetMonthlyState()
    {
        MonthlyGachaPurchasePrice = 0;
        MonthlySlotProfitPrice = 0;
    }

    public void ResetDailyState()
    {
        TodaySlotExecuteCount = 0;
    }

    /// <summary>
    /// 単発ガチャを回す
    /// </summary>
    public GachaProbability? RollGachaOnce()
    {
        MonthlyGachaPurchasePrice += MasterManager.SettingMaster.PricePerGachaOnce;
        return GachaManager.Instance.Roll();
    }

    /// <summary>
    /// 単発確定ガチャを回す
    /// </summary>
    public GachaProbability RollGachaOnceCertain()
    {
        MonthlyGachaPurchasePrice += MasterManager.SettingMaster.PricePerGachaOnceCertain;
        return GachaManager.Instance.RollWithoutNone();
    }

    /// <summary>
    /// 10連ガチャを回す
    /// </summary>
    public List<GachaProbability?> RollGachaTenTimes()
    {
        const int pickCount = 10;
        MonthlyGachaPurchasePrice += MasterManager.SettingMaster.PricePerGachaTenTimes;
        return Enumerable.Range(0, pickCount).Select(_ => GachaManager.Instance.Roll()).ToList();
    }

    /// <summary>
    /// スロットを回す
    /// </summary>
    public SlotExecuteResult ExecuteSlot(Slot slot)
    {
        if (TodaySlotExecuteCount >= MasterManager.SettingMaster.UserSlotExecuteLimitPerDay)
        {
            throw new AppException("今日はもう回せないぞカス");
        }

        var price = slot.ExecutePrice;
        var result = slot.Execute();
        var reward = (int)(NumberUtility.GetPercentFromPermillage(result.ResultRatePermillage) * price);
        MonthlySlotProfitPrice += reward - price;
        TodaySlotExecuteCount++;
        return result;
    }
}
