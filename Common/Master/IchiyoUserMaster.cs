using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

/// <summary>
/// Ichiyo用ユーザー名マスタ
/// Discord User ID → AIに伝える名前のマッピング
/// </summary>
[MasterTable("ichiyo_user")]
public class IchiyoUserMaster : MasterTable<string, IchiyoUser>
{
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class IchiyoUser : MasterRecord<string>
{
    /// <summary>
    /// Discord User ID
    /// </summary>
    [field: MasterStringValue("user_id")]
    public override string Key { get; }

    /// <summary>
    /// AIに伝える名前
    /// </summary>
    [field: MasterStringValue("name")]
    public string Name { get; }
}
