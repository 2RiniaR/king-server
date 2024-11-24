using Approvers.King.Common;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events;

public class MonthlyResetPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var purchaseRankingUsers = await app.Users
            .OrderByDescending(user => user.MonthlyPurchase)
            .Take(MasterManager.SettingMaster.PurchaseInfoRankingViewUserCount)
            .Select(x => x.DeepCopy())
            .ToListAsync();
        var slotRewardRankingUsers = await app.Users
            .OrderByDescending(user => user.MonthlySlotReward)
            .Take(MasterManager.SettingMaster.PurchaseInfoRankingViewUserCount)
            .Select(x => x.DeepCopy())
            .ToListAsync();

        await app.Users.ForEachAsync(user => user.ResetMonthlyState());
        await app.SaveChangesAsync();

        await SendSummaryAsync(purchaseRankingUsers, slotRewardRankingUsers);
    }

    private async Task SendSummaryAsync(IReadOnlyList<User> purchaseRankingUsers, IReadOnlyList<User> slotRewardRankingUsers)
    {
        var embed = new EmbedBuilder()
            .WithColor(Color.LightOrange)
            .WithTitle(Format.Bold($"{IssoUtility.SmileStamp} †今月も貢げカス† {IssoUtility.SmileStamp}"))
            .WithDescription("月が変わったから課金額をリセットした")
            .AddField("先月の課金額ランキング", RankingUtility.CreatePurchaseView(purchaseRankingUsers))
            .AddField("先月の利益ランキング", RankingUtility.CreateSlotRewardView(slotRewardRankingUsers))
            .WithCurrentTimestamp()
            .Build();

        await DiscordManager.GetMainChannel().SendMessageAsync(embed: embed);
    }
}
