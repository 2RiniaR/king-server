using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class SilentCommandPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        SilentManager.SetSilent(Message.Author.Id, MasterManager.SilentTimeSpan);
        var message = string.Format(
            MasterManager.SilentCommandReplyMessage,
            MentionUtils.MentionUser(Message.Author.Id));
        await Message.ReplyAsync(message);
    }
}