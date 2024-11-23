using System.Diagnostics.CodeAnalysis;

namespace Approvers.King.Common;

public class SlotItemMaster : MasterTable<string, SlotItem>;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class SlotItem : MasterRecord<string>
{
    public override string Key => Id;

    [field: MasterStringValue("id")]
    public string Id { get; }

    [field: MasterStringValue("format")]
    public string Format { get; }

    [field: MasterIntValue("return_rate_permillage")]
    public int ReturnRatePermillage { get; }
}
