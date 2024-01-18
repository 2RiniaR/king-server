using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class GachaRateUpdatePresenter : SchedulerJobPresenterBase
{
    private static readonly string IssoSmileStamp = "<:isso_smile:1081501060369236069>";

    protected override async Task MainAsync()
    {
        // 排出確率を変える
        GachaManager.Instance.ShuffleRareReplyRate();
        GachaManager.Instance.ShuffleMessageRates();

        // 名前を更新する
        var guild = DiscordManager.Client.GetGuild(SettingManager.DiscordTargetGuildId);

        await guild.CurrentUser.ModifyAsync(x =>
            x.Nickname = $"{GachaManager.Instance.RareReplyRate:P0}の確率でわかってくれる創造主");

        // 排出率を投稿する
        var records = GachaManager.Instance.ReplyMessageTable
            .OrderByDescending(x => x.Rate)
            .Select(x => (x.Message, x.Rate.ToString("P0")));
        var embed = new EmbedBuilder()
            .WithTitle(
                $"{IssoSmileStamp}{IssoSmileStamp}{IssoSmileStamp} 本日のいっそう {IssoSmileStamp}{IssoSmileStamp}{IssoSmileStamp}")
            .WithColor(new Color(0xf1, 0xc4, 0x0f))
            .WithDescription($"本日は {Discord.Format.Bold($"{GachaManager.Instance.RareReplyRate:P0}")} の確率で反応します")
            .AddField("排出確率", Format.Table(records));

        await guild.GetTextChannel(SettingManager.DiscordMainChannelId)
            .SendMessageAsync(embed: embed.Build());
    }
}
