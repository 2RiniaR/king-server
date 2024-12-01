using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public static class GachaUtility
{
    public static EmbedBuilder GetInfoEmbedBuilder(Gacha gacha)
    {
        var minProbability = Multiplier.FromPercent(1);
        var records = gacha.GachaItems
            .OrderByDescending(x => x.Probability)
            .Where(x => x.Probability > minProbability)
            .Select(x => (x.RandomMessage?.Content ?? MessageConst.MissingMessage, x.Probability.Rate.ToString("P0")));
        return new EmbedBuilder()
            .WithTitle(
                $"{IssoUtility.SmileStamp}{IssoUtility.SmileStamp}{IssoUtility.SmileStamp} 本日のいっそう {IssoUtility.SmileStamp}{IssoUtility.SmileStamp}{IssoUtility.SmileStamp}")
            .WithColor(new Color(0xf1, 0xc4, 0x0f))
            .WithDescription($"本日は {Format.Bold($"{gacha.HitProbability.Rate:P0}")} の確率で反応します")
            .AddField("排出確率", DiscordMessageUtility.Table(records));
    }

    public static string CreateRankingView(IReadOnlyList<User> rankingUsers)
    {
        var embedBuilder = new StringBuilder();
        var order = 1;
        foreach (var user in rankingUsers)
        {
            var scoreText = Math.Min(999_999_999, user.MonthlyGachaPurchasePrice).ToString("N0");
            var whiteSpace = Math.Max(0, 11 - scoreText.Length);
            var line =
                Format.Code($"#{order:D2} - {"".PadLeft(whiteSpace, ' ')}{scoreText}†カス†（税込）") + "  " +
                MentionUtils.MentionUser(user.DiscordId);
            embedBuilder.AppendLine(line);
            order++;
        }

        return embedBuilder.ToString();
    }
}
