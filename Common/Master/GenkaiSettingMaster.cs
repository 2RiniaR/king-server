namespace Approvers.King.Common;

/// <summary>
/// 限界様の設定値
/// </summary>
public class GenkaiSettingMaster : MasterTable<string, Setting>
{
    /// <summary>
    /// 何日以内に投稿があればアクティブメンバーとみなすか
    /// </summary>
    public int ActiveRoleThresholdDays => int.Parse(Find(nameof(ActiveRoleThresholdDays))!.Value);

    /// <summary>
    /// アクティブメンバーにだけ付与するロールのID
    /// </summary>
    public ulong ActiveRoleId => ulong.Parse(Find(nameof(ActiveRoleId))!.Value);
}
