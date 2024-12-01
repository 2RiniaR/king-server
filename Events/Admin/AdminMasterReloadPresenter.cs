using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

/// <summary>
/// マスタデータのリセットを行うイベント
/// </summary>
public class AdminMasterReloadPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await Message.ReplyAsync("マスターをリロードするぞ");
        await MasterManager.FetchAsync();
        await Message.ReplyAsync("マスターをリロードしたぞ");
    }
}
