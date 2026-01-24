#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class IchiyoSettingMaster : MasterTable<string, Setting>
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
    /// AIの役割・ルールを定義するシステムプロンプト
    /// </summary>
    public string SystemPrompt => GetString(nameof(SystemPrompt));

    /// <summary>
    /// 汎用エラーメッセージ
    /// </summary>
    public string ErrorMessage => GetString(nameof(ErrorMessage));

    /// <summary>
    /// タイムアウト時のメッセージ
    /// </summary>
    public string TimeoutMessage => GetString(nameof(TimeoutMessage));

    /// <summary>
    /// 空入力時のメッセージ
    /// </summary>
    public string EmptyInputMessage => GetString(nameof(EmptyInputMessage));

    /// <summary>
    /// context-window limitに達した時のメッセージ
    /// </summary>
    public string ContextLimitMessage => GetString(nameof(ContextLimitMessage));
}
