using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class BirthPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        var guild = DiscordManager.Client.GetGuild(SettingManager.DiscordTargetGuildId);
        var message = Format.Italics(Format.Bold(MasterManager.BirthdayMessage));
        await guild.GetTextChannel(SettingManager.DiscordMainChannelId).SendMessageAsync(message);
    }
}
