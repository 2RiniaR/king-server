using System.Text;
using Discord;

namespace Approvers.King.Common;

public static class DiscordMessageUtility
{
    public static string Table(IEnumerable<(string key, string value)> records)
    {
        var recordList = records.ToList();
        var maxLength = recordList.Max(r => r.key.Length);
        var sb = new StringBuilder();
        foreach (var (key, value) in recordList)
        {
            sb.AppendLine($"| {value.PadLeft(maxLength)} | {Format.Sanitize(key)}");
        }

        return Format.Code(sb.ToString());
    }
}
