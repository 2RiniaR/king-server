using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common.Instances;

public class EyesBotInstance : DiscordBotInstanceBase
{
    public EyesBotInstance() : base(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged |
                         GatewayIntents.MessageContent |
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
    }
}
