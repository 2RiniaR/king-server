using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class SlotExecutePresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var user = await app.FindOrCreateUserAsync(Message.Author.Id);
        var result = user.ExecuteSlot();

        await app.SaveChangesAsync();

        var slotMessage = await Message.ReplyAsync(CreateSlotMessage(result, 0));
        await Task.Delay(TimeSpan.FromSeconds(1));
        var reelCount = result.ReelItems.Length;
        for (var i = 0; i < reelCount; i++)
        {
            await slotMessage.ModifyAsync(prop => prop.Content = CreateSlotMessage(result, i + 1));
            if (i < reelCount - 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }
        }
    }

    private static string CreateSlotMessage(SlotExecuteResult result, int openReelCount)
    {
        var sb = new StringBuilder();

        var reelCount = result.ReelItems.Length;
        for (var i = 0; i < reelCount; i++)
        {
            if (i < openReelCount)
            {
                sb.Append(result.ReelItems[i].Format);
            }
            else
            {
                sb.Append(MasterManager.SettingMaster.SlotReelRollingFormat);
            }
        }

        sb.Append(MasterManager.SettingMaster.SlotLeverFormat);

        return sb.ToString();
    }
}
