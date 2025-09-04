using System.Text;
using Discord;

namespace Approvers.King.Common;

public static class DiscordFormatUtility
{
    public static string Smile => MasterManager.IssoSettingMaster.CommonSmileFormat;
    public static string Missing => MasterManager.IssoSettingMaster.CommonMissingMessage;

    public static string Repeat(this string value, int count, string separator = "")
    {
        return string.Join(separator, EnumerableUtility.Repeat(value, count));
    }

    public static string Custom(this string value, string formatter)
    {
        var isBold = formatter.Contains("b");
        var isItalic = formatter.Contains("i");
        var isUnderline = formatter.Contains("u");
        var isStrike = formatter.Contains("s");
        var isCode = formatter.Contains("c");
        var isSpoiler = formatter.Contains("p");

        var result = value;
        if (isBold) result = Format.Bold(result);
        if (isItalic) result = Format.Italics(result);
        if (isUnderline) result = Format.Underline(result);
        if (isStrike) result = Format.Strikethrough(result);
        if (isCode) result = Format.Code(result);
        if (isSpoiler) result = Format.Spoiler(result);

        return result;
    }

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
