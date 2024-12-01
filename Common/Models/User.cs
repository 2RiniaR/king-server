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
    public GachaItem? RollGachaOnce(Gacha gacha)
    {
        MonthlyGachaPurchasePrice += MasterManager.SettingMaster.PricePerGachaOnce;
        return gacha.RollOnce();
    }

    /// <summary>
    /// 単発確定ガチャを回す
    /// </summary>
    public GachaItem RollGachaOnceCertain(Gacha gacha)
    {
        MonthlyGachaPurchasePrice += MasterManager.SettingMaster.PricePerGachaOnceCertain;
        return gacha.RollOnceCertain();
    }

    /// <summary>
    /// 10連ガチャを回す
    /// </summary>
    public List<GachaItem?> RollGachaTenTimes(Gacha gacha)
    {
        const int pickCount = 10;
        MonthlyGachaPurchasePrice += MasterManager.SettingMaster.PricePerGachaTenTimes;
        return Enumerable.Range(0, pickCount).Select(_ => gacha.RollOnce()).ToList();
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
        var reward = (int)(price * result.ResultRate);
        MonthlySlotProfitPrice += reward - price;
        TodaySlotExecuteCount++;
        return result;
    }
}
