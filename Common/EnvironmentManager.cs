using Microsoft.Extensions.Configuration;

namespace Approvers.King.Common;

public class EnvironmentManager
{
    private static IConfigurationRoot Configuration { get; } = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("./Environment/appsettings.json", true)
        .AddEnvironmentVariables()
        .AddUserSecrets<EnvironmentManager>(true)
        .Build();

    public static string DiscordSecret => Get("DiscordSecret");
    public static ulong DiscordTargetGuildId => ulong.Parse(Get("DiscordTargetGuildId"));
    public static ulong DiscordMainChannelId => ulong.Parse(Get("DiscordMainChannelId"));
    public static string GoogleCredentialFilePath => Get("GoogleCredentialFilePath");
    public static string GoogleMasterSheetId => Get("GoogleMasterSheetId");

    private static string Get(string name)
    {
        return Configuration[name] ??
               throw new Exception($"Environment variable {name} is not set.");
    }
}
