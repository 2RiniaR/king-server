using Approvers.King.Common;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events;

public class SlotRankingPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var selfUser = await app.FindOrCreateUserAsync(Message.Author.Id);
        var users = await app.Users
            .OrderByDescending(user => user.MonthlySlotProfitPrice)
            .Take(MasterManager.SettingMaster.PurchaseInfoRankingViewUserCount)
            .ToListAsync();

        await SendReplyAsync(selfUser, users);
    }

    private async Task SendReplyAsync(User selfUser, IReadOnlyList<User> users)
    {
        var embed = new EmbedBuilder()
            .WithColor(Color.LightOrange)
            .AddField("おまえの今月のスロット利益額", $"{selfUser.MonthlySlotProfitPrice:N0}†カス†（税込）", inline: true)
            .AddField("スロット利益額ランキング", SlotUtility.CreateRankingView(users))
            .WithCurrentTimestamp()
            .Build();

        await Message.ReplyAsync(embed: embed);
    }
}
