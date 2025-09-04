using System.Diagnostics.CodeAnalysis;

namespace Approvers.King.Common;

[MasterTable("angry")]
public class IssoAngryMaster : MasterTable<string, IssoAngry>
{
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class IssoAngry : MasterRecord<string>
{
    [field: MasterStringValue("target")]
    public override string Key { get; }

    [field: MasterStringValue("word")]
    public string Word { get; }

    [field: MasterIntValue("order")]
    public int Order { get; }
}
