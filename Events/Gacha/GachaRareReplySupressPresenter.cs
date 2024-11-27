using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class GachaRareReplySupressPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        var silentTimeSpan = NumberUtility.GetTimeSpanFromMilliseconds(MasterManager.SettingMaster.SilentDuration);
        SilentManager.SetSilent(Message.Author.Id, silentTimeSpan);
        var message = string.Format(
            MasterManager.SettingMaster.SilentReplyMessage,
            MentionUtils.MentionUser(Message.Author.Id));
        await Message.ReplyAsync(message);
    }
}
