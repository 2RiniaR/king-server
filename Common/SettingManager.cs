using Microsoft.Extensions.Configuration;

namespace Approvers.King.Common;

public class SettingManager
{
    private static IConfigurationRoot Configuration { get; } = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true)
        .AddEnvironmentVariables()
        .AddUserSecrets<SettingManager>(true)
        .Build();

    public static string Get(string name)
    {
        return Configuration[name] ??
               throw new Exception($"Environment variable {name} is not set.");
    }
}
