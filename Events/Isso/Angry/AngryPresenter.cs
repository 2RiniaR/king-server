using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Isso;

public class AngryPresenter : DiscordMessagePresenterBase
{
    /// <summary>
    /// ミスリードモードで実行するかどうか（通常マッチがない場合にtrue）
    /// </summary>
    public bool IsMisleadMode { get; set; }

    protected override async Task MainAsync()
    {
        IssoAngry? matchedAngry;

        if (IsMisleadMode)
        {
            // ミスリードモード: orderが大きい順に抽選を行う
            matchedAngry = TryMisleadLottery();
            if (matchedAngry == null)
            {
                // 抽選に外れた場合は何もしない
                return;
            }
        }
        else
        {
            // 通常モード: メッセージ内容からマッチするエントリを探す
            var messageContent = Message.Content.ToLower();
            matchedAngry = MasterManager.IssoAngryMaster
                .GetAll(angry => messageContent.Contains(angry.Key.ToLower()))
                .OrderByDescending(angry => angry.Order)
                .First();
        }

        var replyMessage = $"今 ***\"{matchedAngry.Word}\"*** って言ったか？{MasterManager.IssoSettingMaster.CommonAngryFormat}";
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
