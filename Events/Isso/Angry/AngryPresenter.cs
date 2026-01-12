using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Isso;

public class AngryPresenter : DiscordMessagePresenterBase
{
    /// <summary>
    /// 外部から指定されたAngryエントリ（ミスリード発動時に使用）
    /// </summary>
    public IssoAngry? SpecifiedAngry { get; set; }

    protected override async Task MainAsync()
    {
        IssoAngry matchedAngry;

        if (SpecifiedAngry != null)
        {
            // 外部から指定されたエントリを使用（ミスリード発動時）
            matchedAngry = SpecifiedAngry;
        }
        else
        {
            // メッセージ内容からマッチするエントリを探す（通常発動時）
            var messageContent = Message.Content.ToLower();
            matchedAngry = MasterManager.IssoAngryMaster
                .GetAll(angry => messageContent.Contains(angry.Key.ToLower()))
                .OrderByDescending(angry => angry.Order)
                .First();
        }

        var replyMessage = $"今 ***\"{matchedAngry.Word}\"*** って言ったか？{MasterManager.IssoSettingMaster.CommonAngryFormat}";
        await SendReplyAsync(replyMessage);
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
