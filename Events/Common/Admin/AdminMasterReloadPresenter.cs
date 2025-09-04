using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Common;

/// <summary>
/// マスタデータのリセットを行うイベント
/// </summary>
public class AdminMasterReloadPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await MasterManager.FetchAsync();
        await Message.ReplyAsync("done(ドゥーン)");
    }
}
