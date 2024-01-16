using Approvers.King.Common;
using Approvers.King.Events;
using Discord.WebSocket;

namespace Approvers.King.Triggers;

public static class DiscordTrigger
{
    public static void RegisterEvents()
    {
        DiscordManager.Client.MessageReceived += OnMessageReceived;
    }

    private static async Task OnMessageReceived(SocketMessage message)
    {
        // botは弾く
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        if (userMessage.MentionedUsers.Any(x => x.Id == DiscordManager.Client.CurrentUser.Id))
        {
            if (MasterManager.SilentTriggerMessages.Any(userMessage.Content.Contains))
            {
                // 黙らせる
                await DiscordManager.ExecuteAsync<SilentCommandPresenter>(userMessage);
                return;
            }

            if (MasterManager.GachaTriggerMessages.Any(userMessage.Content.Contains))
            {
                // 10連ガチャ
                await DiscordManager.ExecuteAsync<GachaCommandPresenter>(userMessage);
                return;
            }

            // 返信
            await DiscordManager.ExecuteAsync<InteractReplyPresenter>(userMessage);
            return;
        }

        // 発言
        await DiscordManager.ExecuteAsync<RareReplyPresenter>(userMessage);
    }
}
