using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public static class RankingUtility
{
    public static string CreatePurchaseView(IReadOnlyList<User> rankingUsers)
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

        return embedBuilder.ToString();
    }

    public static string CreateSlotRewardView(IReadOnlyList<User> rankingUsers)
    {
        var embedBuilder = new StringBuilder();
        var order = 1;
        foreach (var user in rankingUsers)
        {
            var scoreText = Math.Min(999_999_999, user.MonthlySlotReward).ToString("N0");
            var whiteSpace = Math.Max(0, 11 - scoreText.Length);
            var line =
                Format.Code($"#{order:D2} - {"".PadLeft(whiteSpace, ' ')}{scoreText}†カス†（税込）") + "  " +
                MentionUtils.MentionUser(user.DiscordID);
            embedBuilder.AppendLine(line);
            order++;
        }

        return embedBuilder.ToString();
    }
}
