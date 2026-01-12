using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Isso;

public class AngryPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        var messageContent = Message.Content.ToLower();

        // まず通常のマッチを確認
        var matchedAngry = MasterManager.IssoAngryMaster
            .GetAll(angry => messageContent.Contains(angry.Key.ToLower()))
            .OrderByDescending(angry => angry.Order)
            .FirstOrDefault();

        if (matchedAngry != null)
        {
            // 通常マッチがあればそのメッセージを返して終了
            await SendAngryReplyAsync(matchedAngry);
            return;
        }

        // 何も引っ掛からなかった場合、ミスリード抽選を行う
        var misleadAngry = TryMisleadLottery();
        if (misleadAngry != null)
        {
            await SendAngryReplyAsync(misleadAngry);
        }
    }

    private async Task SendAngryReplyAsync(IssoAngry angry)
    {
        var replyMessage = $"今 ***\"{angry.Word}\"*** って言ったか？{MasterManager.IssoSettingMaster.CommonAngryFormat}";
        await SendReplyAsync(replyMessage);
    }

    /// <summary>
    /// ミスリード抽選を行い、当たったエントリを返す（外れた場合はnull）
    /// </summary>
    private static IssoAngry? TryMisleadLottery()
    {
        var angryEntries = MasterManager.IssoAngryMaster
            .GetAll()
            .OrderByDescending(angry => angry.Order);

        foreach (var angry in angryEntries)
        {
            if (angry.MisleadPermillage <= 0) continue;

            var probability = Multiplier.FromPermillage(angry.MisleadPermillage);
            if (RandomManager.IsHit(probability))
            {
                return angry;
            }
        }

        return null;
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
