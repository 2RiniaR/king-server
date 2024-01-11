using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class RareReplyPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        if (RandomUtility.GetRandomFloat(1f) > MasterManager.ReplyRate) return;

        await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(MasterManager.ReplyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(MasterManager.TypingMaxDelay)));
            await Message.ReplyAsync(PickRandomMessage());
        }
    }

    private static string PickRandomMessage()
    {
        var totalRate = MasterManager.ReplyMessages.Sum(x => x.rate);
        var value = RandomUtility.GetRandomFloat(totalRate);

        foreach (var (rate, message) in MasterManager.ReplyMessages)
        {
            if (value < rate) return message;

            value -= rate;
        }

        return MasterManager.ReplyMessages[^1].message;
    }
}