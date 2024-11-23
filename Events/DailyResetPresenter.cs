using Approvers.King.Common;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events;

public class DailyResetPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();
        await app.Users.ForEachAsync(user => user.ResetDailySlotExecuteCount());
        await app.SaveChangesAsync();

        // 排出確率を変える
        GachaManager.Instance.RefreshMessageTable();
        GachaManager.Instance.ShuffleRareReplyRate();
        GachaManager.Instance.ShuffleMessageRates();
        await GachaManager.Instance.SaveAsync();

        // 名前を更新する
        await DiscordManager.GetClientUser().ModifyAsync(x => x.Nickname = $"{GachaManager.Instance.RareReplyRate:P0}の確率でわかってくれる創造主");

        // 排出率を投稿する
        await DiscordManager.GetMainChannel().SendMessageAsync(embed: GachaUtility.GetInfoEmbedBuilder().Build());
    }
}
