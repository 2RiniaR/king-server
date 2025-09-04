using Approvers.King.Common.Instances;

namespace Approvers.King.Common;

public class DiscordManager : Singleton<DiscordManager>
{
    private IssoBotInstance _issoBot = null!;
    public static IssoBotInstance IssoBot => Instance._issoBot;

    private EyesBotInstance _eyesBot = null!;
    public static EyesBotInstance EyesBot => Instance._eyesBot;

    public static async Task InitializeAsync()
    {
        Instance._issoBot = new IssoBotInstance();
        Instance._eyesBot = new EyesBotInstance();

        await Task.WhenAll(
            Instance._issoBot.InitializeAsync(),
            Instance._eyesBot.InitializeAsync()
        );
    }
}
