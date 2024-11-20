using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class MasterReloadPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await Message.ReplyAsync("マスターをリロードするぞ");
        await MasterManager.FetchAsync();
        await Message.ReplyAsync("マスターをリロードしたぞ");

        await UpdateGachaTableAsync();
    }

    private async Task UpdateGachaTableAsync()
    {
        var beforeTable = GachaManager.Instance.ReplyMessageTable.Select(x => x.Message.Id).ToHashSet();
        GachaManager.Instance.RefreshMessageTable();
        var afterTable = GachaManager.Instance.ReplyMessageTable.Select(x => x.Message.Id).ToHashSet();

        // テーブルに差分がある場合は排出率を更新する
        var hasDiff = beforeTable.SetEquals(afterTable);
        if (hasDiff == false)
        {
            GachaManager.Instance.ShuffleMessageRates();

            // 排出率を投稿する
            var guild = DiscordManager.Client.GetGuild(EnvironmentManager.DiscordTargetGuildId);
            await guild.GetTextChannel(EnvironmentManager.DiscordMainChannelId)
                .SendMessageAsync(embed: GachaUtility.GetInfoEmbedBuilder().Build());
        }
    }
}