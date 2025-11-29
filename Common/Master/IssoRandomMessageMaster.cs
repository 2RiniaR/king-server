using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class IssoRandomMessageMaster : MasterTable<string, IssoRandomMessage>;

public enum RandomMessageType
{
    Unknown,
    GeneralReply,
    GachaFailed,
    VoiceDiffJoin,
    VoiceDiffLeave,
}

/// <summary>
/// discordに送信するランダムなメッセージ
/// </summary>
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class IssoRandomMessage : MasterRecord<string>
{
    public override string Key => Id;

    [field: MasterStringValue("id")]
    public string Id { get; }

    [field: MasterEnumValue("type", typeof(RandomMessageType))]
    public RandomMessageType Type { get; }

    [field: MasterStringValue("content")]
    public string Content { get; }
}
