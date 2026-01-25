using Approvers.King.Events.Ichiyo;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common.Instances;

public class IchiyoBotInstance : DiscordBotInstanceBase
{
    public IchiyoBotInstance() : base(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.MessageContent |
                         GatewayIntents.Guilds |
                         GatewayIntents.GuildMessages
    })
    {
    }

    public override string DisplayName => "Ichiyo";

    protected override string GetToken()
    {
        return EnvironmentManager.DiscordSecretIchiyo;
    }

    public void RegisterEvents()
    {
        Client.MessageReceived += message =>
        {
            OnMessageReceived(message);
            return Task.CompletedTask;
        };
    }

    private void OnMessageReceived(SocketMessage message)
    {
        // botからのメッセージは無視
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot)
            return;

        var clientUser = GetClientUser();

        // botへのリプライの場合 → セッション継続（メンションより優先）
        if (IsReplyToBot(userMessage, clientUser))
        {
            if (userMessage.Reference?.MessageId.IsSpecified != true)
                return;

            var referencedMessageId = userMessage.Reference.MessageId.Value;

            // セッションを検索
            var sessionId = IchiyoSessionStore.Instance.GetSessionId(referencedMessageId);
            if (sessionId == null)
            {
                // セッションが見つからない場合は新規セッションとして処理
                ExecuteMessageEventAsync<IchiyoChatPresenter>(userMessage).Run();
                return;
            }

            ExecuteMessageEventAsync<IchiyoChatPresenter>(userMessage, presenter =>
            {
                presenter.ResumeSessionId = sessionId;
                return Task.CompletedTask;
            }).Run();
            return;
        }

        // メンションされた場合 → 新規セッション
        if (IsMentioned(userMessage, clientUser))
        {
            ExecuteMessageEventAsync<IchiyoChatPresenter>(userMessage).Run();
        }
    }

    /// <summary>
    /// メッセージがこのbotにメンションしているかチェック
    /// </summary>
    private static bool IsMentioned(SocketUserMessage message, SocketGuildUser clientUser)
    {
        return message.MentionedUsers.Any(u => u.Id == clientUser.Id);
    }

    /// <summary>
    /// メッセージがこのbotの発言へのリプライかチェック
    /// </summary>
    private bool IsReplyToBot(SocketUserMessage message, SocketGuildUser clientUser)
    {
        if (message.Reference?.MessageId.IsSpecified != true)
            return false;

        var referencedMessageId = message.Reference.MessageId.Value;

        // キャッシュからリプライ先のメッセージを取得して確認
        var referencedMessage = message.Channel.GetCachedMessage(referencedMessageId);
        if (referencedMessage != null)
        {
            return referencedMessage.Author.Id == clientUser.Id;
        }

        // キャッシュになくてもセッションストアにあれば、それはbotへのリプライ
        return IchiyoSessionStore.Instance.HasSession(referencedMessageId);
    }
}
