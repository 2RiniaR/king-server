using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class LoxyIgnoreEnglishMaster : MasterTable<string, LoxyIgnoreEnglish>
{
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class LoxyIgnoreEnglish : MasterRecord<string>
{
    [field: MasterStringValue("key")]
    public override string Key { get; }

    [field: MasterStringValue("value")]
    public string Value { get; }
}
