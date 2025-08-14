using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class AngryPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        var messageContent = Message.Content.ToLower();
        
        // すべてのAngryエントリをチェックし、最もorderが高いものを適用
        // (Program.csで既に一致チェック済みのため、ここではFirstOrDefaultで取得)
        var matchedAngry = MasterManager.AngryMaster
            .GetAll(angry => messageContent.Contains(angry.Key.ToLower()))
            .OrderByDescending(angry => angry.Order)
            .First();

        var replyMessage = $"今 ***\"{matchedAngry.Word}\"*** って言ったか？{MasterManager.SettingMaster.CommonAngryFormat}";
        await SendReplyAsync(replyMessage);
    }

    private async Task SendReplyAsync(string message)
    {
        var replyMaxDelay = NumberUtility.GetSecondsFromMilliseconds(MasterManager.SettingMaster.ReplyMaxDuration);
        await Task.Delay(TimeSpan.FromSeconds(RandomManager.GetRandomFloat(replyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            var typingMaxDelay =
                NumberUtility.GetSecondsFromMilliseconds(MasterManager.SettingMaster.TypingMaxDuration);
            await Task.Delay(TimeSpan.FromSeconds(RandomManager.GetRandomFloat(typingMaxDelay)));
            await Message.ReplyAsync(message);
        }
    }
}