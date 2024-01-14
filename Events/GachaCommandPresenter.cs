using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class GachaCommandPresenter : DiscordMessagePresenterBase
{
    private const int PickCount = 10;

    protected override async Task MainAsync()
    {
        var results = Enumerable.Range(0, PickCount).Select(_ => Pick());

        var builder = new StringBuilder();
        builder.AppendLine($"↓↓↓ いっそう{PickCount}連おみくじ ↓↓↓");
        foreach (var result in results)
        {
            builder.AppendLine(result != null ? Discord.Format.Bold($"・{result}") : Discord.Format.Code("x"));
        }

        await Message.ReplyAsync(builder.ToString());
    }

    private static string? Pick()
    {
        if (RandomUtility.IsHit(MasterManager.ReplyRate) == false) return null;
        return MessageUtility.PickRandomMessage();
    }
}