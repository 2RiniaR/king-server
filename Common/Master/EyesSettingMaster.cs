#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class EyesSettingMaster : SettingMasterBase
{
    /// <summary>
    /// 反応確率
    /// </summary>
    public int HitPermillage => GetInt(nameof(HitPermillage));

    /// <summary>
    /// 反応に必要なメッセージなし時間（ミリ秒）
    /// </summary>
    public int RequireSilenceTime => GetInt(nameof(RequireSilenceTime));

    /// <summary>
    /// 反応までの最小の遅延時間（ミリ秒）
    /// </summary>
    public int MinStandByTime => GetInt(nameof(MinStandByTime));

    /// <summary>
    /// 反応までの最大の遅延時間（ミリ秒）
    /// </summary>
    public int MaxStandByTime => GetInt(nameof(MaxStandByTime));

    /// <summary>
    /// 反応メッセージ
    /// </summary>
    public string MessageContent => GetString(nameof(MessageContent));
}
