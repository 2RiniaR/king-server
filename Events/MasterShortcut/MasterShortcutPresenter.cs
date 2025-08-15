using Approvers.King.Common;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Events;

public class MasterShortcutPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        var masterDataUrl = MasterManager.SettingMaster.MasterDataUrl;
        await Message.ReplyAsync(masterDataUrl);
    }
}