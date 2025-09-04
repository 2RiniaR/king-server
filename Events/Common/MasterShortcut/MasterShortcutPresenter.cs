using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Common;

public class MasterShortcutPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        var masterDataUrl = MasterManager.CommonSettingMaster.MasterDataUrl;
        await Message.ReplyAsync(masterDataUrl);
    }
}
