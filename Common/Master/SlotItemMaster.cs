using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class SlotItemMaster : MasterTable<string, SlotItem>;

/// <summary>
/// スロットの出目
/// </summary>
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class SlotItem : MasterRecord<string>
{
    public override string Key => Id;

    [field: MasterStringValue("id")]
    public string Id { get; }

    /// <summary>
    /// discordに送信する絵文字フォーマット
    /// </summary>
    [field: MasterStringValue("format")]
    public string Format { get; }

    /// <summary>
    /// 出目が揃った時の、掛け金に対するキャッシュバック倍率
    /// </summary>
    [field: MasterIntValue("return_rate_permillage")]
    public int ReturnRatePermillage { get; }

    /// <summary>
    /// 次に同じ出目が確定する確率(千分率)
    /// </summary>
    [field: MasterIntValue("repeat_permillage")]
    public int RepeatPermillage { get; }
}
