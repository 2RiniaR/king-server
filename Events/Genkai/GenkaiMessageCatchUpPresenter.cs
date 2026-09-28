using Approvers.King.Common;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events.Genkai;

/// <summary>
/// 停止中に投稿されたメッセージを検索して、最終投稿日時に反映するイベント
/// </summary>
public class GenkaiMessageCatchUpPresenter : PresenterBase
{
    /// <summary>
    /// 検索APIで1回に指定できる投稿者数の上限
    /// </summary>
    private const int MaxAuthorsPerSearch = 100;

    /// <summary>
    /// メンバーを検索APIの上限人数ずつに分け、各グループで起点より後の投稿を探す
    /// </summary>
    protected override async Task MainAsync()
    {
        var guild = DiscordManager.GenkaiBot.GetGuild();
        await guild.DownloadUsersAsync();

        await using var app = AppService.CreateSession();

        // 稼働中の投稿はイベントで記録済みなので、一番新しい記録より後だけを探せば停止中の投稿が揃う
        var thresholdStart = TimeManager.GetNow().AddDays(-MasterManager.GenkaiSettingMaster.ActiveRoleThresholdDays);
        var lastRecordedAt = await app.Users.MaxAsync(x => x.LastMessageAt);
        var minMessageId = SnowflakeUtils.ToSnowflake(lastRecordedAt > thresholdStart ? lastRecordedAt.Value : thresholdStart);

        var searchCount = 0;
        var updatedCount = 0;
        foreach (var authorIds in guild.Users.Where(x => x.IsBot == false).Select(x => x.Id).Chunk(MaxAuthorsPerSearch))
        {
            var remainingAuthorIds = authorIds.ToHashSet();
            while (remainingAuthorIds.Count > 0)
            {
                var result = await guild.SearchMessagesAsync(new SearchGuildMessages
                {
                    AuthorIds = remainingAuthorIds.ToArray(),
                    MinMessageId = minMessageId,
                    IncludeNsfw = true
                });
                searchCount++;
                if (result.Messages.Count == 0) break;

                // 新しい順に並んでいるので、各投稿者が最初に出てきたメッセージがその人の最新の投稿になる
                foreach (var message in result.Messages)
                {
                    if (remainingAuthorIds.Remove(message.Author.Id) == false) continue;

                    var user = await app.FindOrCreateUserAsync(message.Author.Id);
                    user.LastMessageAt = message.Timestamp.LocalDateTime;
                    updatedCount++;
                }
            }
        }

        await app.SaveChangesAsync();
        LogManager.Log($"[Genkai] {searchCount}回の検索で{updatedCount}人の最終投稿日時を更新");
    }
}
