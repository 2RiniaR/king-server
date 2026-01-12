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
    /// 全エントリのmislead_permillageを集計して一回で抽選する
    /// </summary>
    private static IssoAngry? TryMisleadLottery()
    {
        var candidates = MasterManager.IssoAngryMaster
            .GetAll()
            .Where(angry => angry.MisleadPermillage > 0)
            .ToList();

        if (candidates.Count == 0) return null;

        // 0-999の乱数を生成
        var roll = RandomManager.GetRandomInt(1000);

        // 各エントリの確率範囲をチェック
        var threshold = 0;
        foreach (var angry in candidates)
        {
            threshold += angry.MisleadPermillage;
            if (roll < threshold)
            {
                return angry;
            }
        }

        // 外れ
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
