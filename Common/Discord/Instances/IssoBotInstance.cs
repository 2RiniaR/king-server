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
}
