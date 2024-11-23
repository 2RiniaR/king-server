using System.Text;
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
        var embedBuilder = new StringBuilder();
        var order = 1;
        foreach (var user in rankingUsers)
        {
            var scoreText = Math.Min(999_999_999, user.MonthlyPurchase).ToString("N0");
            var whiteSpace = Math.Max(0, 11 - scoreText.Length);
            var line =
                Format.Code($"#{order:D2} - {"".PadLeft(whiteSpace, ' ')}{scoreText}†カス†（税込）") + "  " +
                MentionUtils.MentionUser(user.DiscordID);
            embedBuilder.AppendLine(line);
            order++;
        }

        var embed = new EmbedBuilder()
            .WithColor(Color.LightOrange)
            .AddField("おまえの今月の課金額", $"{selfUser.MonthlyPurchase:N0}†カス†（税込）", inline: true)
            .AddField("ランキング", embedBuilder.ToString())
            .WithCurrentTimestamp()
            .Build();

        await Message.ReplyAsync(embed: embed);
    }
}
