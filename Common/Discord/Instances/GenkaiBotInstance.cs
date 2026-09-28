using Approvers.King.Events.Genkai;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common.Instances;

/// <summary>
/// アクティブメンバー用ロールを保守する限界様
/// </summary>
public class GenkaiBotInstance : DiscordBotInstanceBase
{
    /// <summary>
    /// メンバー一覧とロールをキャッシュし続けるためにGuildMembersを、投稿を記録するためにGuildMessagesを購読する
    /// </summary>
    public GenkaiBotInstance() : base(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.Guilds |
                         GatewayIntents.GuildMembers |
                         GatewayIntents.GuildMessages
    })
    {
    }

    /// <summary>
    /// ログに出す名前
    /// </summary>
    public override string DisplayName => "Genkai";

    /// <summary>
    /// 限界様のbotトークン
    /// </summary>
    protected override string GetToken()
    {
        return EnvironmentManager.DiscordSecretGenkai;
    }

    /// <summary>
    /// 投稿のたびに投稿者の最終投稿日時を記録する
    /// </summary>
    public void RegisterEvents()
    {
        Client.MessageReceived += message =>
        {
            OnMessageReceived(message);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// botとシステムメッセージは投稿とみなさない
    /// </summary>
    private void OnMessageReceived(SocketMessage message)
    {
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        ExecuteMessageEventAsync<GenkaiMessageRecordPresenter>(userMessage).Run();
    }
}
