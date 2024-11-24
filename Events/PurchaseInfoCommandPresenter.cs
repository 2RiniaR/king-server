using Approvers.King.Common;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events;

public class PurchaseInfoCommandPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var selfUser = await app.FindOrCreateUserAsync(Message.Author.Id);
        var purchaseRankingUsers = await app.Users
            .OrderByDescending(user => user.MonthlyPurchase)
            .Take(MasterManager.SettingMaster.PurchaseInfoRankingViewUserCount)
            .ToListAsync();
        var slotRewardRankingUsers = await app.Users
            .OrderByDescending(user => user.MonthlySlotReward)
            .Take(MasterManager.SettingMaster.PurchaseInfoRankingViewUserCount)
            .ToListAsync();

        await SendReplyAsync(selfUser, purchaseRankingUsers, slotRewardRankingUsers);
    }

    private async Task SendReplyAsync(User selfUser, IReadOnlyList<User> purchaseRankingUsers, IReadOnlyList<User> slotRewardRankingUsers)
    {
        var embed = new EmbedBuilder()
            .WithColor(Color.LightOrange)
            .AddField("おまえの今月の課金額", $"{selfUser.MonthlyPurchase:N0}†カス†（税込）", inline: true)
            .AddField("課金額ランキング", RankingUtility.CreatePurchaseView(purchaseRankingUsers))
            .AddField("利益ランキング", RankingUtility.CreateSlotRewardView(slotRewardRankingUsers))
            .WithCurrentTimestamp()
            .Build();

        await Message.ReplyAsync(embed: embed);
    }
}
