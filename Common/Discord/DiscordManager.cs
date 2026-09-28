using Approvers.King.Common.Instances;

namespace Approvers.King.Common;

public class DiscordManager : Singleton<DiscordManager>
{
    private IssoBotInstance _issoBot = null!;
    public static IssoBotInstance IssoBot => Instance._issoBot;

    private EyesBotInstance _eyesBot = null!;
    public static EyesBotInstance EyesBot => Instance._eyesBot;

    private LoxyBotInstance _loxyBot = null!;
    public static LoxyBotInstance LoxyBot => Instance._loxyBot;

    private GenkaiBotInstance _genkaiBot = null!;
    public static GenkaiBotInstance GenkaiBot => Instance._genkaiBot;

    public static async Task InitializeAsync()
    {
        Instance._issoBot = new IssoBotInstance();
        Instance._eyesBot = new EyesBotInstance();
        Instance._loxyBot = new LoxyBotInstance();
        Instance._genkaiBot = new GenkaiBotInstance();

        await Task.WhenAll(
            Instance._issoBot.InitializeAsync(),
            Instance._eyesBot.InitializeAsync(),
            Instance._loxyBot.InitializeAsync(),
            Instance._genkaiBot.InitializeAsync()
        );
    }
}
