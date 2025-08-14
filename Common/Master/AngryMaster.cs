using System.Diagnostics.CodeAnalysis;

namespace Approvers.King.Common;

[MasterTable("angry")]
public class AngryMaster : MasterTable<string, Angry>
{
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class Angry : MasterRecord<string>
{
    [field: MasterStringValue("target")]
    public override string Key { get; }

    [field: MasterStringValue("word")]
    public string Word { get; }

    [field: MasterIntValue("order")]
    public int Order { get; }
}
