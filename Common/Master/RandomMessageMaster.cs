using System.Diagnostics.CodeAnalysis;

namespace Approvers.King.Common;

public class RandomMessageMaster : MasterTable<string, RandomMessage>;

public enum RandomMessageType
{
    Unknown,
    GeneralReply,
    GachaFailed,
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class RandomMessage : MasterRecord<string>
{
    public override string Key => Id;

    [field: MasterStringValue("id")]
    public string Id { get; }

    [field: MasterEnumValue("type", typeof(RandomMessageType))]
    public RandomMessageType Type { get; }

    [field: MasterStringValue("content")]
    public string Content { get; }
}
