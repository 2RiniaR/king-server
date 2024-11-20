using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class RareReplyPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        if (SilentManager.IsSilent(Message.Author.Id) ||
            Message.Channel.Id != EnvironmentManager.DiscordMainChannelId) return;

        await using var app = AppService.CreateSession();
        var user = await app.FindOrCreateUserAsync(Message.Author.Id);

        var message = user.RollGachaOnce();
        if (message != null)
        {
            await SendReplyAsync(message.RandomMessage?.Content ?? MessageConst.MissingMessage);
        }

        await app.SaveChangesAsync();
    }

    private async Task SendReplyAsync(string message)
    {
        var replyMaxDelay = NumberUtility.GetSecondsFromMilliseconds(MasterManager.SettingMaster.ReplyMaxDuration);
        await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(replyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            var typingMaxDelay =
                NumberUtility.GetSecondsFromMilliseconds(MasterManager.SettingMaster.TypingMaxDuration);
            await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(typingMaxDelay)));
            await Message.ReplyAsync(message);
        }
    }
}
