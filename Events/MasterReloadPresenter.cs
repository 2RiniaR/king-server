using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class MasterReloadPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await Message.ReplyAsync("マスターをリロードするぞ");
        await MasterManager.FetchAsync();
        await Message.ReplyAsync("マスターをリロードしたぞ");
    }
}
