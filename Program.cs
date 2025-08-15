using Approvers.King.Common;
using Approvers.King.Events;
using Discord.WebSocket;

namespace Approvers.King;

public static class Program
{
    /// <summary>
    /// エントリーポイント
    /// </summary>
    private static void Main(string[] args)
    {
        BuildAsync(args).GetAwaiter().GetResult();
    }

    private static async Task BuildAsync(string[] args)
    {
        await InitializeModulesAsync();
        await InitializeStatesAsync();
        RegisterEvents();

        // 永久に待つ
        await Task.Delay(-1);
    }

    /// <summary>
    /// 共通基盤系を初期化する
    /// </summary>
    private static async Task InitializeModulesAsync()
    {
        TimeManager.Instance.Initialize();
        await MasterManager.FetchAsync();
        SchedulerManager.Initialize();
        await DiscordManager.InitializeAsync();
    }

    private static async Task InitializeStatesAsync()
    {
        await using var app = AppService.CreateSession();

        var gacha = await app.GetDefaultGachaAsync();
        if (gacha.GachaItems.Count == 0)
        {
            gacha.ShuffleMessageRates();
        }

        if (gacha.HitProbability == Multiplier.Zero)
        {
            gacha.ShuffleRareReplyRate();
        }

        await app.SaveChangesAsync();
    }

    /// <summary>
    /// 全てのイベントを登録する
    /// </summary>
    private static void RegisterEvents()
    {
        DiscordManager.Client.MessageReceived += message =>
        {
            OnMessageReceived(message);
            return Task.CompletedTask;
        };

        SchedulerManager.RegisterDaily<DailyResetPresenter>(TimeManager.DailyResetTime);
        SchedulerManager.RegisterMonthly<MonthlyResetPresenter>(TimeManager.MonthlyResetDay, TimeManager.DailyResetTime);
        SchedulerManager.RegisterOn<SlotConditionRefreshPresenter>(x => x.Minute is 0);
    }

    /// <summary>
    /// トリガーとなる文言を含んでいるか
    /// </summary>
    private static bool IsContainsTriggerPhrase(string content, TriggerType triggerType)
    {
        return MasterManager.TriggerPhraseMaster
            .GetAll(x => x.TriggerType == triggerType)
            .Any(x => content.Contains(x.Phrase));
    }

    private static void OnMessageReceived(SocketMessage message)
    {
        // botは弾く
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        // チャンネルがutil_onlyの場合、MessageLink以外の機能を無効化
        var channelId = userMessage.Channel.Id.ToString();
        var channel = MasterManager.ChannelMaster.Find(channelId);
        var isUtilOnlyChannel = channel?.IsUtilOnly ?? false;

        // メッセージリンクが含まれている場合の処理
        if (userMessage.Content.Contains("discord.com/channels/") || userMessage.Content.Contains("discordapp.com/channels/"))
        {
            DiscordManager.ExecuteAsync<MessageLinkPresenter>(userMessage).Run();
        }

        // util_onlyチャンネルの場合、これ以降の処理をスキップ
        if (isUtilOnlyChannel) return;

        if (userMessage.MentionedUsers.Any(x => x.Id == DiscordManager.Client.CurrentUser.Id))
        {
            if (message.Content.EndsWith("reload"))
            {
                // マスタデータをリロード
                DiscordManager.ExecuteAsync<AdminMasterReloadPresenter>(userMessage).Run();
                return;
            }

            if (TryExecuteMarugame(userMessage))
            {
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaRanking))
            {
                // ガチャランキングの表示
                DiscordManager.ExecuteAsync<GachaRankingPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.SlotRanking))
            {
                // スロットランキングの表示
                DiscordManager.ExecuteAsync<SlotRankingPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaExecute))
            {
                // 10連ガチャ
                DiscordManager.ExecuteAsync<GachaCommandPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaGet))
            {
                // 排出率を投稿する
                DiscordManager.ExecuteAsync<GachaInfoCommandPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.SlotExecute))
            {
                // スロットを回す
                DiscordManager.ExecuteAsync<SlotExecutePresenter>(userMessage).Run();
                return;
            }

            // 返信
            DiscordManager.ExecuteAsync<GachaInteractReplyPresenter>(userMessage).Run();
            return;
        }

        // 発言（Angry機能）
        if (TryExecuteAngry(userMessage))
        {
            return;
        }
        
        // 発言
        DiscordManager.ExecuteAsync<GachaRareReplyPresenter>(userMessage).Run();
    }

    private static bool TryExecuteAngry(SocketUserMessage userMessage)
    {
        var messageContent = userMessage.Content.ToLower();
        
        // すべてのAngryエントリをチェックし、一致するものがあるかを確認
        var hasMatch = MasterManager.AngryMaster
            .GetAll(angry => messageContent.Contains(angry.Key.ToLower()))
            .Any();

        if (hasMatch)
        {
            DiscordManager.ExecuteAsync<AngryPresenter>(userMessage).Run();
            return true;
        }

        return false;
    }

    private static bool TryExecuteMarugame(SocketUserMessage userMessage)
    {
        // 「丸亀製麺」の次にある改行以降が対象
        var marugameTrigger =
            MasterManager.TriggerPhraseMaster.FirstOrDefault(x => x.TriggerType == TriggerType.Marugame)?.Phrase ?? "";

        var marugameIndex =
            userMessage.Content.IndexOf(marugameTrigger, StringComparison.InvariantCulture);
        if (marugameIndex < 0) return false;

        var subs = userMessage.Content[(marugameIndex + marugameTrigger.Length)..];
        var contentIndex = subs.IndexOf('\n');
        if (contentIndex < 0) return false;

        // 丸亀製麺
        DiscordManager.ExecuteAsync<MarugamePresenter>(userMessage, presenter =>
        {
            presenter.Content = subs[(contentIndex + 1)..];
            return Task.CompletedTask;
        }).Run();

        return true;
    }
}
