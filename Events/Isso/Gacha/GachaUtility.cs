using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Isso;

using F = DiscordFormatUtility;

public static class GachaUtility
{
    public static EmbedBuilder GetInfoEmbedBuilder(Gacha gacha)
    {
        var minProbability = Multiplier.FromPercent(1);
        var records = gacha.GachaItems
            .OrderByDescending(x => x.Probability)
            .Where(x => x.Probability > minProbability)
            .Select(x => (x.RandomMessage?.Content ?? F.Missing, x.Probability.Rate.ToString("P0")));
        return new EmbedBuilder()
            .WithTitle($"{F.Smile.Repeat(3)} 本日のいっそう {F.Smile.Repeat(3)}")
            .WithColor(new Color(0xf1, 0xc4, 0x0f))
            .WithDescription($"本日は {$"{gacha.HitProbability.Rate:P0}".Custom("b")} の確率で反応します")
            .AddField("排出確率", F.Table(records));
    }

    public static string CreateRankingView(IReadOnlyList<User> rankingUsers)
    {
        var embedBuilder = new StringBuilder();
        var order = 1;
        foreach (var user in rankingUsers)
        {
            var scoreText = Math.Min(999_999_999, user.MonthlyGachaPurchasePrice).ToString("N0");
            var whiteSpace = Math.Max(0, 11 - scoreText.Length);
            var line = $"#{order:D2} - {"".PadLeft(whiteSpace, ' ')}{scoreText}†カス†（税込）".Custom("c") + "  " + MentionUtils.MentionUser(user.DiscordId);
            embedBuilder.AppendLine(line);
            order++;
        }

        return embedBuilder.ToString();
    }
}
