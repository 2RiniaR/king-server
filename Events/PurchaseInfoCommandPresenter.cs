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
        var rankingUsers = await app.Users
            .OrderByDescending(user => user.MonthlyPurchase)
            .Take(MasterManager.SettingMaster.PurchaseInfoRankingViewUserCount)
            .ToListAsync();

        await SendReplyAsync(selfUser, rankingUsers);
    }

    private async Task SendReplyAsync(User selfUser, IReadOnlyList<User> rankingUsers)
    {
        var embed = new EmbedBuilder()
            .WithColor(Color.LightOrange)
            .AddField("おまえの今月の課金額", $"{selfUser.MonthlyPurchase:N0}†カス†（税込）", inline: true)
            .AddField("ランキング", PurchaseUtility.CreateRankingView(rankingUsers))
            .WithCurrentTimestamp()
            .Build();

        await Message.ReplyAsync(embed: embed);
    }
}
