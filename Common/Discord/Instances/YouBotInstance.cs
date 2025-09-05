using Approvers.King.Events.You;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common.Instances;

public class YouBotInstance : DiscordBotInstanceBase
{
    public YouBotInstance() : base(new DiscordSocketConfig
    {
        MessageCacheSize = 100,
        GatewayIntents = GatewayIntents.Guilds |
                         GatewayIntents.GuildMembers |
                         GatewayIntents.GuildMessages |
                         GatewayIntents.MessageContent
    })
    {
    }

    public override string DisplayName => "You";

    protected override string GetToken()
    {
        return EnvironmentManager.DiscordSecretYou;
    }

    public void RegisterEvents()
    {
        Client.MessageDeleted += async (messageCache, channelCache) =>
        {
            var message = await messageCache.GetOrDownloadAsync();
            if (message == null) return;

            var channel = await channelCache.GetOrDownloadAsync();
            if (channel == null) return;

            OnMessageDeleted(message, channel);
        };
    }

    private void OnMessageDeleted(IMessage message, IMessageChannel channel)
    {
        ExecuteMessageDeleteEventAsync<YouSendPresenter>(message, channel).Run();
    }
}
