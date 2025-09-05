using Approvers.King.Common;

namespace Approvers.King.Events.You;

public class YouSendPresenter : DiscordMessageDeletePresenterBase
{
    protected override async Task MainAsync()
    {
        var now = TimeManager.GetNow();
        if (IsHit(now) == false)
        {
            return;
        }

        AppCache.Instance.YouLastSendTime = now;
        await Channel.SendMessageAsync(MasterManager.YouSettingMaster.MessageContent);
    }

    private bool IsHit(DateTime now)
    {
        // クールタイム中は発動しない
        var coolTime = TimeSpan.FromMilliseconds(MasterManager.YouSettingMaster.SendCoolTime);
        var lastSend = AppCache.Instance.YouLastSendTime;
        if (lastSend.HasValue && now - lastSend < coolTime)
        {
            return false;
        }

        // 一定時間以上前のメッセージが削除された場合は、発動しない
        var requireRecent = TimeSpan.FromMilliseconds(MasterManager.YouSettingMaster.RequireRecentTime);
        var messageAge = now - Message.CreatedAt.LocalDateTime;
        if (messageAge >= requireRecent)
        {
            return false;
        }

        // Isso, Eyesのメッセージが削除されたときは、確定で発動
        if (Message.Author.Id == DiscordManager.IssoBot.GetClientUser().Id ||
            Message.Author.Id == DiscordManager.EyesBot.GetClientUser().Id)
        {
            return true;
        }

        // それ以外のbotメッセージが削除された場合は、発動しない
        if (Message.Author.IsBot)
        {
            return false;
        }

        // 誰かのメッセージが削除された時、確率で発動
        var probability = Multiplier.FromPermillage(MasterManager.YouSettingMaster.HitPermillage);
        return RandomManager.IsHit(probability);
    }
}
