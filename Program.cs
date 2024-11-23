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
        await MasterManager.FetchAsync();

        await GachaManager.Instance.LoadAsync();
        SlotManager.Instance.LoadMaster();
        SchedulerManager.Initialize();
        await DiscordManager.InitializeAsync();

        if (GachaManager.Instance.IsTableEmpty)
        {
            // 起動時にデータがない場合、ガチャ確率を初期化する
            await new GachaRateUpdatePresenter().RunAsync();
        }

        DiscordManager.Client.MessageReceived += OnMessageReceived;

        SchedulerManager.RegisterDaily<GachaRateUpdatePresenter>(TimeManager.DailyResetTime);
        SchedulerManager.RegisterYearly<BirthPresenter>(TimeManager.Birthday + TimeManager.DailyResetTime +
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

    private static async Task OnMessageReceived(SocketMessage message)
    {
        // botは弾く
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        if (userMessage.MentionedUsers.Any(x => x.Id == DiscordManager.Client.CurrentUser.Id))
        {
            if (message.Content.EndsWith("reload"))
            {
                // マスタデータをリロード
                await DiscordManager.ExecuteAsync<MasterReloadPresenter>(userMessage);
                return;
            }

            if (await TryExecuteMarugame(userMessage)) return;

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.Silent))
            {
                // 黙らせる
                await DiscordManager.ExecuteAsync<SilentCommandPresenter>(userMessage);
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.PurchaseGet))
            {
                // 課金情報の表示
                await DiscordManager.ExecuteAsync<PurchaseInfoCommandPresenter>(userMessage);
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaExecute))
            {
                // 10連ガチャ
                await DiscordManager.ExecuteAsync<GachaCommandPresenter>(userMessage);
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaGet))
            {
                // 排出率を投稿する
                await DiscordManager.ExecuteAsync<GachaInfoCommandPresenter>(userMessage);
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.SlotExecute))
            {
                // スロットを回す
                await DiscordManager.ExecuteAsync<SlotExecutePresenter>(userMessage);
                return;
            }

            // 返信
            await DiscordManager.ExecuteAsync<InteractReplyPresenter>(userMessage);
            return;
        }

        // 発言
        await DiscordManager.ExecuteAsync<RareReplyPresenter>(userMessage);
    }

    private static async Task<bool> TryExecuteMarugame(SocketUserMessage userMessage)
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
        await DiscordManager.ExecuteAsync<MarugamePresenter>(userMessage, presenter =>
        {
            presenter.Content = subs[(contentIndex + 1)..];
            return Task.CompletedTask;
        });
        return true;
    }
}
