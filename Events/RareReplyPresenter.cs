﻿using Approvers.King.Common;
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
            await Message.ReplyAsync(MessageUtility.PickRandomMessage());
        }
    }
}