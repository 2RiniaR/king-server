using System.Text;
using Discord;

namespace Approvers.King.Common;

public abstract class PresenterBase
{
    public async Task RunAsync()
    {
        try
        {
            await MainAsync();
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync("========================================\n");
            Console.Error.Write(e);
        }
    }

    protected abstract Task MainAsync();

    public static Embed ErrorEmbed(string message)
    {
        return new EmbedBuilder()
            .WithColor(Color.Red)
            .WithTitle("⚠ エラー")
            .WithDescription(message)
            .WithCurrentTimestamp()
            .Build();
    }

    public static class Format
    {
        public static string Table(IEnumerable<(string key, string value)> records)
        {
            var recordList = records.ToList();
            var maxLength = recordList.Max(r => r.key.Length);
            var sb = new StringBuilder();
            foreach (var (key, value) in recordList)
            {
                sb.AppendLine($"| {value.PadLeft(maxLength)} | {Discord.Format.Sanitize(key)}");
            }

            return Discord.Format.Code(sb.ToString());
        }
    }
}