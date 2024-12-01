using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

/// <summary>
/// スロットを実行するイベント
/// </summary>
public class SlotExecutePresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();

        var user = await app.FindOrCreateUserAsync(Message.Author.Id);
        var slot = await app.GetDefaultSlotAsync();
        var result = user.ExecuteSlot(slot);

        await app.SaveChangesAsync();

        var slotMessage = await Message.ReplyAsync(CreateSlotMessage(result, 0));
        var purchaseMessage =
            await slotMessage.Channel.SendMessageAsync(CreatePurchaseMessage(result, user, false));

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

        await purchaseMessage.ModifyAsync(prop =>
            prop.Content = CreatePurchaseMessage(result, user, true));
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

    private static string CreatePurchaseMessage(SlotExecuteResult result, User user, bool isOpen)
    {
        var sb = new StringBuilder();

        if (isOpen == false)
        {
            sb.AppendLine("......");
        }
        else
        {
            if (result.IsWin)
            {
                sb.AppendLine(Format.Bold($"Y O U   W I N ! !   x{result.ResultRate.Rate:F1}"));
            }
            else
            {
                sb.AppendLine(Format.Bold($"Y O U   L O S E"));
            }
        }

        if (isOpen == false)
        {
            sb.AppendLine("おまえの今月の利益 → ???");
        }
        else
        {
            sb.AppendLine($"おまえの今月の利益 → {user.MonthlySlotProfitPrice:N0}†カス†（税込）");
        }

        return sb.ToString();
    }
}
