using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Approvers.King.Common;

public class User
{
    [Key]
    public ulong DiscordID { get; set; }
    public int MonthlyPurchase { get; set; }
    
    public void ResetMonthlyPurchase()
    {
        MonthlyPurchase = 0;
    }

    public string? RollGachaOnce()
    {
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaOnce;
        return GachaManager.Instance.Roll();
    }
    
    public string RollGachaOnceCertain()
    {
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaOnceCertain;
        return GachaManager.Instance.RollWithoutNone();
    }
    
    public List<string?> RollGachaTenTimes()
    {
        const int pickCount = 10;
        MonthlyPurchase += MasterManager.SettingMaster.PricePerGachaTenTimes;
        return Enumerable.Range(0, pickCount).Select(_ => GachaManager.Instance.Roll()).ToList();
    }
}
