using Approvers.King.Events;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common.Instances;

public class IssoBotInstance : DiscordBotInstanceBase
{
    public IssoBotInstance() : base(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged |
                         GatewayIntents.MessageContent |
                         GatewayIntents.GuildMembers
    })
    {
    }

    public override string DisplayName => "Isso";

    protected override string GetToken()
    {
        return EnvironmentManager.DiscordSecretIsso;
    }

    public void RegisterEvents()
    {
        Client.MessageReceived += message =>
        {
            OnMessageReceived(message);
            return Task.CompletedTask;
        };
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

    private void OnMessageReceived(SocketMessage message)
    {
        // botは弾く
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        // チャンネルがutil_onlyの場合の判定
        var channelId = userMessage.Channel.Id.ToString();
        var channel = MasterManager.ChannelMaster.Find(channelId);
        var isUtilOnlyChannel = channel?.IsUtilOnly ?? false;

        // メッセージリンクが含まれている場合の処理
        if (userMessage.Content.Contains("discord.com/channels/") || userMessage.Content.Contains("discordapp.com/channels/"))
        {
            ExecuteAsync<MessageLinkPresenter>(userMessage).Run();
        }

        if (userMessage.MentionedUsers.Any(x => x.Id == Client.CurrentUser.Id))
        {
            if (message.Content.EndsWith("reload"))
            {
                // マスタデータをリロード
                ExecuteAsync<AdminMasterReloadPresenter>(userMessage).Run();
                return;
            }

            if (TryExecuteMarugame(userMessage))
            {
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaRanking))
            {
                // ガチャランキングの表示
                ExecuteAsync<GachaRankingPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.SlotRanking))
            {
                // スロットランキングの表示
                ExecuteAsync<SlotRankingPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaExecute))
            {
                // 10連ガチャ
                ExecuteAsync<GachaCommandPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaGet))
            {
                // 排出率を投稿する
                ExecuteAsync<GachaInfoCommandPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.SlotExecute))
            {
                // スロットを回す
                ExecuteAsync<SlotExecutePresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.MasterShortcut))
            {
                // マスターデータのURL表示
                ExecuteAsync<MasterShortcutPresenter>(userMessage).Run();
                return;
            }

            // 返信
            ExecuteAsync<GachaInteractReplyPresenter>(userMessage).Run();
            return;
        }

        // util_onlyチャンネルの場合、メンションなしの機能をスキップ
        if (isUtilOnlyChannel) return;

        // 発言（Angry機能）
        if (TryExecuteAngry(userMessage))
        {
            return;
        }

        // 発言
        ExecuteAsync<GachaRareReplyPresenter>(userMessage).Run();
    }

    private bool TryExecuteAngry(SocketUserMessage userMessage)
    {
        var messageContent = userMessage.Content.ToLower();

        // すべてのAngryエントリをチェックし、一致するものがあるかを確認
        var hasMatch = MasterManager.AngryMaster
            .GetAll(angry => messageContent.Contains(angry.Key.ToLower()))
            .Any();

        if (hasMatch)
        {
            ExecuteAsync<AngryPresenter>(userMessage).Run();
            return true;
        }

        return false;
    }

    private bool TryExecuteMarugame(SocketUserMessage userMessage)
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
        ExecuteAsync<MarugamePresenter>(userMessage, presenter =>
        {
            presenter.Content = subs[(contentIndex + 1)..];
            return Task.CompletedTask;
        }).Run();

        return true;
    }
}
