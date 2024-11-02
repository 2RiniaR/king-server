using Approvers.King.Common;

namespace Approvers.King.Events;

public class GachaRateUpdatePresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        // 排出確率を変える
        GachaManager.Instance.ShuffleRareReplyRate();
        GachaManager.Instance.ShuffleMessageRates();

        // 名前を更新する
        var guild = DiscordManager.Client.GetGuild(EnvironmentManager.DiscordTargetGuildId);
        await guild.CurrentUser.ModifyAsync(x =>
            x.Nickname = $"{GachaManager.Instance.RareReplyRate:P0}の確率でわかってくれる創造主");

        // 排出率を投稿する
        await guild.GetTextChannel(EnvironmentManager.DiscordMainChannelId)
            .SendMessageAsync(embed: GachaUtility.GetInfoEmbedBuilder().Build());
    }
}
