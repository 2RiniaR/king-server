using Approvers.King.Common;

namespace Approvers.King.Events;

public class GachaInfoCommandPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        // 排出率を投稿する
        await DiscordManager.Client
            .GetGuild(SettingManager.DiscordTargetGuildId)
            .GetTextChannel(SettingManager.DiscordMainChannelId)
            .SendMessageAsync(embed: GachaUtility.GetInfoEmbedBuilder().Build());
    }
}
