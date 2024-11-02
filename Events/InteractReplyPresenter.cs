using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class InteractReplyPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        var replyMaxDelay = NumberUtility.GetSecondsFromMilliseconds(MasterManager.SettingMaster.ReplyMaxDuration);
        await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(replyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            var typingMaxDelay = NumberUtility.GetSecondsFromMilliseconds(MasterManager.SettingMaster.TypingMaxDuration);
            await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(typingMaxDelay)));
            await Message.ReplyAsync(GachaManager.Instance.PickMessage());
        }
    }
}
