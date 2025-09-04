#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class CommonSettingMaster : SettingMasterBase
{
    /// <summary>
    /// マスターデータのスプレッドシートURL
    /// </summary>
    public string MasterDataUrl => GetString(nameof(MasterDataUrl));
}
