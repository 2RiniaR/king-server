using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public static class SlotUtility
{
    public static string CreateRankingView(IReadOnlyList<User> rankingUsers)
    {
        if (rankingUsers.Count == 0)
        {
            return "ランキングはまだありません";
        }

        var embedBuilder = new StringBuilder();
        var order = 1;
        foreach (var user in rankingUsers)
        {
            var scoreText = Math.Min(999_999_999, user.MonthlySlotProfitPrice).ToString("N0");
            var whiteSpace = Math.Max(0, 11 - scoreText.Length);
            var line = $"#{order:D2} - {"".PadLeft(whiteSpace, ' ')}{scoreText}†カス†（税込）".Custom("c") + "  " + MentionUtils.MentionUser(user.DiscordId);
            embedBuilder.AppendLine(line);
            order++;
        }

        return embedBuilder.ToString();
    }
}
