using System.Text;
using Approvers.King.Common;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events;

/// <summary>
/// 毎日のリセットを行うイベント
/// </summary>
public class DailyResetPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var slotMaxUsers = await app.Users
            .Where(x => x.TodaySlotExecuteCount >= MasterManager.SettingMaster.UserSlotExecuteLimitPerDay)
            .Select(x => x.DeepCopy())
            .ToListAsync();
        await app.Users.ForEachAsync(user => user.ResetDailyState());

        // 排出確率を変える
        var gacha = await app.GetDefaultGachaAsync();
        gacha.ShuffleRareReplyRate();
        gacha.ShuffleMessageRates();

        await app.SaveChangesAsync();

        // 名前を更新する
        await DiscordManager.GetClientUser().ModifyAsync(x => x.Nickname = $"{gacha.HitProbability.Rate:P0}の確率でわかってくれる創造主");

        // 排出率を投稿する
        await DiscordManager.GetMainChannel().SendMessageAsync(embed: GachaUtility.GetInfoEmbedBuilder(gacha).Build());

        // スロットの実行回数が最大になったユーザーを通知する
        await NotifySlotMaxUsers(slotMaxUsers);
    }

    private async Task NotifySlotMaxUsers(IReadOnlyList<User> users)
    {
        if (users.Count == 0)
        {
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("**\u2b07\ufe0e\u2b07\ufe0e\u2b07\ufe0e 昨日のスロカス一覧がこちらw \u2b07\ufe0e\u2b07\ufe0e\u2b07\ufe0e**");
        sb.AppendLine();
        foreach (var user in users)
        {
            sb.AppendLine($"- {MentionUtils.MentionUser(user.DiscordId)}");
        }

        await DiscordManager.GetMainChannel().SendMessageAsync(sb.ToString());
    }
}
