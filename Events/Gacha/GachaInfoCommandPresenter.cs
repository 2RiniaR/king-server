using Approvers.King.Common;

namespace Approvers.King.Events;

/// <summary>
/// ガチャの分布を表示するイベント
/// </summary>
public class GachaInfoCommandPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();
        var gacha = await app.GetDefaultGachaAsync();

        // 排出率を投稿する
        await DiscordManager.Client
            .GetGuild(EnvironmentManager.DiscordTargetGuildId)
            .GetTextChannel(EnvironmentManager.DiscordMainChannelId)
            .SendMessageAsync(embed: GachaUtility.GetInfoEmbedBuilder(gacha).Build());
    }
}
