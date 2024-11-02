using System.Diagnostics.CodeAnalysis;

namespace Approvers.King.Common;

public class TriggerPhraseMaster : MasterTable<string, TriggerPhrase>;

public enum TriggerType
{
    Unknown,
    Silent,
    GachaExecute,
    GachaGet,
    Marugame,
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class TriggerPhrase : MasterRecord<string>
{
    public override string Key => Id;
    
    [field: MasterStringValue("id")]
    public string Id { get; }
    
    [field: MasterEnumValue("trigger_type", typeof(TriggerType))]
    public TriggerType TriggerType { get; }
    
    [field: MasterStringValue("phrase")]
    public string Phrase { get; }
}
