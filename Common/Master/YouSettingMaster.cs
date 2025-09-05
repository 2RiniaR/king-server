#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class YouSettingMaster : MasterTable<string, Setting>
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
    /// 反応確率
    /// </summary>
    public int HitPermillage => GetInt(nameof(HitPermillage));

    /// <summary>
    /// 反応するメッセージの時間制限
    /// </summary>
    public int RequireRecentTime => GetInt(nameof(RequireRecentTime));

    /// <summary>
    /// クールタイム
    /// </summary>
    public int SendCoolTime => GetInt(nameof(SendCoolTime));

    /// <summary>
    /// 反応メッセージ
    /// </summary>
    public string MessageContent => GetString(nameof(MessageContent));
}
