using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Isso;

using F = DiscordFormatUtility;

/// <summary>
/// 確定ガチャを回すイベント
/// </summary>
public class GachaInteractReplyPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();
        var user = await app.FindOrCreateUserAsync(Message.Author.Id);
        var gacha = await app.GetDefaultGachaAsync();

        var message = user.RollGachaOnceCertain(gacha);
        await SendReplyAsync(message.RandomMessage?.Content ?? F.Missing);

        await app.SaveChangesAsync();
    }

    private async Task SendReplyAsync(string message)
    {
        var replyMaxDelay = NumberUtility.GetSecondsFromMilliseconds(MasterManager.IssoSettingMaster.ReplyMaxDuration);
        await Task.Delay(TimeSpan.FromSeconds(RandomManager.GetRandomFloat(replyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            var typingMaxDelay =
                NumberUtility.GetSecondsFromMilliseconds(MasterManager.IssoSettingMaster.TypingMaxDuration);
            await Task.Delay(TimeSpan.FromSeconds(RandomManager.GetRandomFloat(typingMaxDelay)));
            await Message.ReplyAsync(message);
        }
    }
}
