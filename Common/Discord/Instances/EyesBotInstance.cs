using Approvers.King.Events;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common.Instances;

public class EyesBotInstance : DiscordBotInstanceBase
{
    public EyesBotInstance() : base(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.Guilds |
                         GatewayIntents.GuildMembers |
                         GatewayIntents.GuildMessageTyping
    })
    {
    }

    public override string DisplayName => "Eyes";

    protected override string GetToken()
    {
        return EnvironmentManager.DiscordSecretEyes;
    }

    public void RegisterEvents()
    {
        // この辺のキャッシュ周りめんどいので共通化するかも
        Client.UserIsTyping += async (userCache, channelCache) =>
        {
            var user = userCache.HasValue ? userCache.Value : await Client.GetUserAsync(userCache.Id);
            if (user == null) return;

            ISocketMessageChannel? channel = null;
            if (channelCache is { HasValue: true, Value: ISocketMessageChannel socketChannel })
            {
                channel = socketChannel;
            }
            else
            {
                var discordChannel = Client.GetChannel(channelCache.Id);
                channel = discordChannel as ISocketMessageChannel;
            }

            if (channel == null) return;

            OnUserTyping(user as SocketUser ?? Client.GetUser(user.Id), channel);
        };
    }

    private void OnUserTyping(SocketUser? user, ISocketMessageChannel channel)
    {
        if (user == null || user.IsBot)
        {
            return;
        }

        ExecuteTypingEventAsync<EyesSendPresenter>(user, channel).Run();
    }
}
