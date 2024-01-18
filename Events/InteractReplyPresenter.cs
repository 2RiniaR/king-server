using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class InteractReplyPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(MasterManager.ReplyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(MasterManager.TypingMaxDelay)));
            await Message.ReplyAsync(GachaManager.Instance.PickMessage());
        }
    }
}