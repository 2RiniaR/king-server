using Approvers.King.Common;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events.Genkai;

/// <summary>
/// アクティブメンバー用ロールの付与状態を最終投稿日時に合わせるイベント
/// </summary>
public class GenkaiActiveRoleRefreshPresenter : SchedulerJobPresenterBase
{
    /// <summary>
    /// 付与状態が食い違うメンバーにだけ付け外しする
    /// </summary>
    protected override async Task MainAsync()
    {
        var roleId = MasterManager.GenkaiSettingMaster.ActiveRoleId;
        var thresholdStart = TimeManager.GetNow().AddDays(-MasterManager.GenkaiSettingMaster.ActiveRoleThresholdDays);

        await using var app = AppService.CreateSession();
        var activeUserIds = (await app.Users
                .Where(x => x.LastMessageAt >= thresholdStart)
                .Select(x => x.DiscordId)
                .ToListAsync())
            .ToHashSet();

        // botの投稿は記録しないので、botは常に剥奪側になる
        var mismatchedMembers = DiscordManager.GenkaiBot.GetGuild().Users
            .Where(x => activeUserIds.Contains(x.Id) != x.Roles.Any(role => role.Id == roleId));
        foreach (var member in mismatchedMembers)
        {
            if (activeUserIds.Contains(member.Id))
            {
                await member.AddRoleAsync(roleId);
            }
            else
            {
                await member.RemoveRoleAsync(roleId);
            }
        }
    }
}
