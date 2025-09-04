using System.Text;
using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Isso;

/// <summary>
/// 丸亀構文を出力するイベント
/// </summary>
public class MarugamePresenter : DiscordMessagePresenterBase
{
    public string? Content { get; set; }

    protected override async Task MainAsync()
    {
        if (string.IsNullOrEmpty(Content))
        {
            return;
        }

        var message = ConvertMessageToVertical(Content).Custom("c");
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
