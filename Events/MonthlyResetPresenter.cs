using Approvers.King.Common;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events;

public class MonthlyResetPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var summaryUsers = await app.Users
            .OrderByDescending(user => user.MonthlyPurchase)
            .Take(MasterManager.SettingMaster.PurchaseInfoRankingViewUserCount)
            .Select(x => x.DeepCopy())
            .ToListAsync();

        await app.Users.ForEachAsync(user => user.ResetMonthlyPurchase());
        await app.SaveChangesAsync();

        await SendSummaryAsync(summaryUsers);
    }

    private async Task SendSummaryAsync(IReadOnlyList<User> rankingUsers)
    {
        var embed = new EmbedBuilder()
            .WithColor(Color.LightOrange)
            .WithTitle(Format.Bold($"{IssoUtility.SmileStamp} †今月も貢げカス† {IssoUtility.SmileStamp}"))
            .WithDescription("月が変わったから課金額をリセットした")
            .AddField("先月のランキング", PurchaseUtility.CreateRankingView(rankingUsers))
            .WithCurrentTimestamp()
            .Build();

        await DiscordManager.GetMainChannel().SendMessageAsync(embed: embed);
    }
}
