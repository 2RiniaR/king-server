using Approvers.King.Common;

namespace Approvers.King.Events;

public class BirthPresenter : SchedulerJobPresenterBase
{
    private static readonly string Message = $"""
                                              {string.Join("", EnumerableUtility.Repeat(IssoUtility.SmileStamp, 16))}
                                              {IssoUtility.SmileStamp}　　　　　　　　　　　　　　　　　　　 {IssoUtility.SmileStamp}
                                              {IssoUtility.SmileStamp}    ***†　誕　生　日　だ　祝　え　カ　ス　†***   {IssoUtility.SmileStamp}
                                              {IssoUtility.SmileStamp}　　　　　　　　　　　　　　　　　　　 {IssoUtility.SmileStamp}
                                              {string.Join("", EnumerableUtility.Repeat(IssoUtility.SmileStamp, 16))}
                                              """;

    protected override async Task MainAsync()
    {
        var guild = DiscordManager.Client.GetGuild(EnvironmentManager.DiscordTargetGuildId);
        await guild.GetTextChannel(EnvironmentManager.DiscordMainChannelId).SendMessageAsync(Message);
    }
}
