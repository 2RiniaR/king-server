using System.Diagnostics.CodeAnalysis;

namespace Approvers.King.Common;

public class SettingMaster : MasterTable<string, Setting>
{
    private string GetString(string key)
    {
        var record = Find(key);
        if (record == null)
        {
            LogManager.LogError("Setting not found: " + key);
            return string.Empty;
        }
        return record.Value;
    }
    
    private int GetInt(string key)
    {
        var value = GetString(key);
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        if (int.TryParse(value, out var result) == false)
        {
            LogManager.LogError("Failed to parse setting: " + key);
            return 0;
        }

        return result;
    }

    public int ReplyMaxDuration => GetInt(nameof(ReplyMaxDuration));
    public int TypingMaxDuration => GetInt(nameof(TypingMaxDuration));
    public int SilentDuration => GetInt(nameof(SilentDuration));
    public int DailyResetTime => GetInt(nameof(DailyResetTime));
    public int BirthdayMonth => GetInt(nameof(BirthdayMonth));
    public int BirthdayDay => GetInt(nameof(BirthdayDay));
    public int MaxRareReplyProbabilityPermillage => GetInt(nameof(MaxRareReplyProbabilityPermillage));
    public int RareReplyProbabilityStepPermillage => GetInt(nameof(RareReplyProbabilityStepPermillage));
    public string SilentReplyMessage => GetString(nameof(SilentReplyMessage));
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class Setting : MasterRecord<string>
{
    [field: MasterStringValue("key")]
    public override string Key { get; }
    
    [field: MasterStringValue("value")]
    public string Value { get; }
}
