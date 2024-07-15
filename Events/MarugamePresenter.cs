using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class MarugamePresenter : DiscordMessagePresenterBase
{
    public string Content { get; set; }

    protected override async Task MainAsync()
    {
        var message = Format.Code(ConvertMessageToVertical(Content));
        await Message.ReplyAsync(message);
    }

    private static string ConvertMessageToVertical(string message)
    {
        var lines = message.Split('\n');
        var maxLength = lines.Max(x => x.Length);

        var sb = new StringBuilder();
        for (var c = 0; c < maxLength; c++)
        {
            foreach (var line in lines.Reverse())
            {
                sb.Append(c < line.Length ? line[c] : '　');
                sb.Append('　');
            }

            if (c < maxLength - 1) sb.Append('\n');
        }

        return sb.ToString();
    }
}
