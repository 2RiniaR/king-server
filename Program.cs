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
        GachaManager.Instance.Initialize();
        SchedulerManager.Initialize();
        await DiscordManager.InitializeAsync();

        // 起動時には強制的にガチャ確率を更新する
        await new GachaRateUpdatePresenter().RunAsync();

        DiscordManager.Client.MessageReceived += OnMessageReceived;
        SchedulerManager.RegisterDaily<GachaRateUpdatePresenter>(MasterManager.DailyResetTime);
        SchedulerManager.RegisterYearly<BirthPresenter>(MasterManager.Birthday + MasterManager.DailyResetTime +
                                                        TimeSpan.FromSeconds(1));

        // 永久に待つ
        await Task.Delay(-1);
    }

    private static async Task OnMessageReceived(SocketMessage message)
    {
        // botは弾く
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        if (userMessage.MentionedUsers.Any(x => x.Id == DiscordManager.Client.CurrentUser.Id))
        {
            if (await TryExecuteMarugame(userMessage)) return;

            if (MasterManager.SilentTriggerMessages.Any(userMessage.Content.Contains))
            {
                // 黙らせる
                await DiscordManager.ExecuteAsync<SilentCommandPresenter>(userMessage);
                return;
            }

            if (MasterManager.GachaTriggerMessages.Any(userMessage.Content.Contains))
            {
                // 10連ガチャ
                await DiscordManager.ExecuteAsync<GachaCommandPresenter>(userMessage);
                return;
            }

            if (MasterManager.GachaInfoTriggerMessages.Any(userMessage.Content.Contains))
            {
                // 排出率を投稿する
                await DiscordManager.ExecuteAsync<GachaInfoCommandPresenter>(userMessage);
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

        var marugameIndex =
            userMessage.Content.IndexOf(MasterManager.MarugameTrigger, StringComparison.InvariantCulture);
        if (marugameIndex < 0) return false;

        var subs = userMessage.Content[(marugameIndex + MasterManager.MarugameTrigger.Length)..];
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
