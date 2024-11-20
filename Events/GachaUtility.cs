using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public static class GachaUtility
{
    public static EmbedBuilder GetInfoEmbedBuilder()
    {
        var records = GachaManager.Instance.ReplyMessageTable
            .OrderByDescending(x => x.Rate)
            .Select(x => (x.Message.Content, x.Rate.ToString("P0")));
        return new EmbedBuilder()
            .WithTitle(
                $"{IssoUtility.SmileStamp}{IssoUtility.SmileStamp}{IssoUtility.SmileStamp} 本日のいっそう {IssoUtility.SmileStamp}{IssoUtility.SmileStamp}{IssoUtility.SmileStamp}")
            .WithColor(new Color(0xf1, 0xc4, 0x0f))
            .WithDescription($"本日は {Format.Bold($"{GachaManager.Instance.RareReplyRate:P0}")} の確率で反応します")
            .AddField("排出確率", DiscordMessageUtility.Table(records));
    }
}
