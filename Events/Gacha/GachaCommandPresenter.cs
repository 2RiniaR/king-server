using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class GachaCommandPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();
        var user = await app.FindOrCreateUserAsync(Message.Author.Id);

        var results = user.RollGachaTenTimes();
        await SendReplyAsync(user, results);

        await app.SaveChangesAsync();
    }

    private async Task SendReplyAsync(User user, IReadOnlyList<GachaProbability?> results)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"↓↓↓ いっそう{results.Count}連おみくじ ↓↓↓");
        foreach (var result in results)
        {
            builder.AppendLine(result != null
                ? Format.Bold(
                    $"・{result.RandomMessage?.Content ?? MessageConst.MissingMessage} ({result.Probability:P0})")
                : Format.Code("x"));
        }

        // 爆死してたら煽る
        if (results.All(x => x == null))
        {
            builder.AppendLine();
            var messages = MasterManager.RandomMessageMaster.GetAll(x => x.Type == RandomMessageType.GachaFailed);
            var failedMessage = RandomManager.PickRandom(messages).Content;
            builder.AppendLine(Format.Bold(Format.Italics(failedMessage)));
        }

        builder.AppendLine();
        builder.AppendLine($"おまえの今月の課金額 → {user.MonthlyGachaPurchasePrice:N0}†カス†（税込）");

        await Message.ReplyAsync(builder.ToString());
    }
}
