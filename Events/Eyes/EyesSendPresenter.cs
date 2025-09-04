using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class EyesSendPresenter : DiscordTypingPresenterBase
{
    protected override async Task MainAsync()
    {
        // 確率抽選
        if (RandomManager.IsHit(Multiplier.FromPermillage(MasterManager.EyesSettingMaster.HitPermillage)) == false)
        {
            return;
        }

        // ランダムな時間待つ
        await Task.Delay(RandomManager.GetRandomInt(
            MasterManager.EyesSettingMaster.MinStandByTime,
            MasterManager.EyesSettingMaster.MaxStandByTime));

        // 一定時間チャンネルにメッセージが投稿されていなければ、メッセージを送信
        var messages = await Channel.GetMessagesAsync(1).FlattenAsync();
        var lastMessage = messages.FirstOrDefault();
        if (lastMessage != null)
        {
            var requireSilence = TimeSpan.FromMilliseconds(MasterManager.EyesSettingMaster.RequireSilenceTime);
            var currentSilence = TimeManager.GetNow() - lastMessage.CreatedAt.LocalDateTime;
            if (currentSilence < requireSilence)
            {
                return;
            }
        }

        await Channel.SendMessageAsync(MasterManager.EyesSettingMaster.MessageContent);
    }
}
