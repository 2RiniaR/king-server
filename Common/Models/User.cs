using System.ComponentModel.DataAnnotations;

namespace Approvers.King.Common;

public class User
{
    [Key] public ulong DiscordID { get; set; }
    public int MonthlyPurchase { get; set; }
    public int TodaySlotExecuteCount { get; set; }

    public User DeepCopy()
    {
        var user = (User)MemberwiseClone();
        return user;
    }

    public void ResetMonthlyPurchase()
    {
        MonthlyPurchase = 0;
    }

    public void ResetDailySlotExecuteCount()
    {
        TodaySlotExecuteCount = 0;
    }

    public GachaProbability? RollGachaOnce()
    {
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaOnce;
        return GachaManager.Instance.Roll();
    }

    public GachaProbability RollGachaOnceCertain()
    {
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaOnceCertain;
        return GachaManager.Instance.RollWithoutNone();
    }

    public List<GachaProbability?> RollGachaTenTimes()
    {
        const int pickCount = 10;
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaTenTimes;
        return Enumerable.Range(0, pickCount).Select(_ => GachaManager.Instance.Roll()).ToList();
    }

    public SlotExecuteResult ExecuteSlot()
    {
        if (TodaySlotExecuteCount >= MasterManager.SettingMaster.UserSlotExecuteLimitPerDay)
        {
            throw new AppException("今日はもう回せないぞカス");
        }

        var price = MasterManager.SettingMaster.PricePerSlotOnce;
        var result = SlotManager.Instance.Execute();
        MonthlyPurchase += price - (int)(NumberUtility.GetPercentFromPermillage(result.ResultRatePermillage) * price);
        TodaySlotExecuteCount++;
        return result;
    }
}
