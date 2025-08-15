using System.Diagnostics.CodeAnalysis;

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