using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

[MasterTable("channel")]
public class ChannelMaster : MasterTable<string, Channel>
{
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class Channel : MasterRecord<string>
{
    [field: MasterStringValue("channel_id")]
    public override string Key { get; }

    [field: MasterBoolValue("is_util_only")]
    public bool IsUtilOnly { get; }
}
