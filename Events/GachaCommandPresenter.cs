using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class GachaCommandPresenter : DiscordMessagePresenterBase
{
    private const int PickCount = 10;

    protected override async Task MainAsync()
    {
        var results = Enumerable.Range(0, PickCount)
            .Select(_ => GachaManager.Instance.TryPickRareReplyMessage())
            .ToList();

        var builder = new StringBuilder();
        builder.AppendLine($"↓↓↓ いっそう{PickCount}連おみくじ ↓↓↓");
        foreach (var result in results)
        {
            builder.AppendLine(result != null ? Format.Bold($"・{result}") : Format.Code("x"));
        }

        if (results.All(x => x == null))
        {
            builder.AppendLine();
            var ridiculeMessage = MasterManager.RandomMessageMaster.GetAll(x => x.Type == RandomMessageType.GachaFailed).PickRandom().Content;
            builder.AppendLine(Format.Bold(Format.Italics(ridiculeMessage)));
        }

        await Message.ReplyAsync(builder.ToString());
    }
}
