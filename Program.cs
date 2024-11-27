using Approvers.King.Common;
using Approvers.King.Events;
using Discord.WebSocket;

namespace Approvers.King;

public class Program
{
    private static void Main(string[] args)
    {
        BuildAsync(args).GetAwaiter().GetResult();
    }

    private static async Task BuildAsync(string[] args)
    {
        TimeManager.Instance.Initialize();
        await MasterManager.FetchAsync();

        await GachaManager.Instance.LoadAsync();
        SlotManager.Instance.LoadMaster();
        SchedulerManager.Initialize();
        await DiscordManager.InitializeAsync();

        if (GachaManager.Instance.IsTableEmpty)
        {
            // 起動時にデータがない場合、ガチャ確率を初期化する
            await new DailyResetPresenter().RunAsync();
        }

        DiscordManager.Client.MessageReceived += message =>
        {
            OnMessageReceived(message);
            return Task.CompletedTask;
        };

        SchedulerManager.RegisterDaily<DailyResetPresenter>(TimeManager.DailyResetTime);
        SchedulerManager.RegisterYearly<DailyResetBirthPresenter>(TimeManager.Birthday + TimeManager.DailyResetTime +
                                                        TimeSpan.FromSeconds(1));
        SchedulerManager.RegisterMonthly<MonthlyResetPresenter>(TimeManager.MonthlyResetDay,
            TimeManager.DailyResetTime);

        // 永久に待つ
        await Task.Delay(-1);
    }

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

        if (userMessage.MentionedUsers.Any(x => x.Id == DiscordManager.Client.CurrentUser.Id))
        {
            if (message.Content.EndsWith("reload"))
            {
                // マスタデータをリロード
                DiscordManager.ExecuteAsync<AdminMasterReloadPresenter>(userMessage).Run();
                return;
            }

            if (TryExecuteMarugame(userMessage)) return;

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.Silent))
            {
                // 黙らせる
                DiscordManager.ExecuteAsync<GachaRareReplySupressPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.PurchaseGet))
            {
                // 課金情報の表示
                DiscordManager.ExecuteAsync<PurchaseShowPresenter>(userMessage).Run();
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

        // 発言
        DiscordManager.ExecuteAsync<GachaRareReplyPresenter>(userMessage).Run();
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
