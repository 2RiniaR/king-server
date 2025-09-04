using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class CommonSettingMaster : MasterTable<string, Setting>
{
    protected string GetString(string key)
    {
        var record = Find(key);
        if (record == null)
        {
            LogManager.LogError("Setting not found: " + key);
            return string.Empty;
        }

        return record.Value;
    }

    protected int GetInt(string key)
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

    /// <summary>
    /// マスターデータのスプレッドシートURL
    /// </summary>
    public string MasterDataUrl => GetString(nameof(MasterDataUrl));
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class Setting : MasterRecord<string>
{
    [field: MasterStringValue("key")]
    public override string Key { get; }

    [field: MasterStringValue("value")]
    public string Value { get; }
}
