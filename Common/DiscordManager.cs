using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common;

public static class DiscordManager
{
    public static readonly DiscordSocketClient Client = new(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMessages,
    });

    public static async Task InitializeAsync()
    {
        Client.Log += OnLog;
        await Client.LoginAsync(TokenType.Bot, SettingManager.DiscordSecret);
        await Client.StartAsync();
        await EventUtility.WaitAsync(h => Client.Ready += h, h => Client.Ready -= h);
    }

    private static Task OnLog(LogMessage content)
    {
        Console.WriteLine(content.ToString());
        return Task.CompletedTask;
    }

    public static async Task ExecuteAsync<T>(SocketUserMessage message, Func<T, Task>? onInitializeAsync = null)
        where T : DiscordMessagePresenterBase, new()
    {
        var presenter = new T { Message = message };
        if (onInitializeAsync != null) await onInitializeAsync.Invoke(presenter);
        await presenter.RunAsync();
    }
}
