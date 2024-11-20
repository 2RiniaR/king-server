using System.ComponentModel.DataAnnotations;

namespace Approvers.King.Common;

public class User
{
    [Key] public ulong DiscordID { get; set; }
    public int MonthlyPurchase { get; set; }

    public void ResetMonthlyPurchase()
    {
        MonthlyPurchase = 0;
    }

    public RandomMessage? RollGachaOnce()
    {
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaOnce;
        return GachaManager.Instance.Roll();
    }

    public RandomMessage RollGachaOnceCertain()
    {
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaOnceCertain;
        return GachaManager.Instance.RollWithoutNone();
    }

    public List<RandomMessage?> RollGachaTenTimes()
    {
        const int pickCount = 10;
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaTenTimes;
        return Enumerable.Range(0, pickCount).Select(_ => GachaManager.Instance.Roll()).ToList();
    }
}
