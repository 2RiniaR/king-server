using Approvers.King.Common;
using Approvers.King.Events;
using Discord.WebSocket;

namespace Approvers.King;

public static class DiscordEntry
{
    public static void RegisterEvents()
    {
        DiscordManager.Client.MessageReceived += OnMessageReceived;
    }

    private static async Task OnMessageReceived(SocketMessage message)
    {
        // botは弾く
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        // 発言
        await DiscordManager.ExecuteAsync<ReplyPresenter>(userMessage);
    }
}