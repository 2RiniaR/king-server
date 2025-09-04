using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common;

public abstract class DiscordBotInstanceBase
{
    public DiscordSocketClient Client { get; }
    public abstract string DisplayName { get; }

    public DiscordBotInstanceBase(DiscordSocketConfig config)
    {
        Client = new DiscordSocketClient(config);
    }

    protected abstract string GetToken();

    public async Task InitializeAsync()
    {
        Client.Log += OnLog;

        await Client.LoginAsync(TokenType.Bot, GetToken());
        await Client.StartAsync();
        await TaskUtility.WaitAsync(h => Client.Ready += h, h => Client.Ready -= h);
    }

    private Task OnLog(LogMessage log)
    {
        Console.WriteLine($"[{DisplayName}] {log}");
        return Task.CompletedTask;
    }

    public SocketGuildUser GetClientUser()
    {
        var guild = Client.GetGuild(EnvironmentManager.DiscordTargetGuildId);
        return guild.CurrentUser;
    }

    public SocketTextChannel GetMainChannel()
    {
        var guild = Client.GetGuild(EnvironmentManager.DiscordTargetGuildId);
        return guild.GetTextChannel(EnvironmentManager.DiscordMainChannelId);
    }

    public async Task ExecuteMessageEventAsync<T>(SocketUserMessage message, Func<T, Task>? onInitializeAsync = null)
        where T : DiscordMessagePresenterBase, new()
    {
        var presenter = new T { Message = message };
        if (onInitializeAsync != null) await onInitializeAsync.Invoke(presenter);
        await presenter.RunAsync();
    }

    public async Task ExecuteTypingEventAsync<T>(SocketUser user, ISocketMessageChannel channel, Func<T, Task>? onInitializeAsync = null)
        where T : DiscordTypingPresenterBase, new()
    {
        var presenter = new T { User = user, Channel = channel };
        if (onInitializeAsync != null) await onInitializeAsync.Invoke(presenter);
        await presenter.RunAsync();
    }
}
