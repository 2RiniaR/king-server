using System.Collections.Concurrent;

namespace Approvers.King.Common;

/// <summary>
/// Ichiyoのセッションをインメモリで管理するストア
/// DiscordメッセージID ↔ ClaudeセッションIDをマッピング
/// </summary>
public class IchiyoSessionStore
{
    private static readonly Lazy<IchiyoSessionStore> LazyInstance = new(() => new IchiyoSessionStore());
    public static IchiyoSessionStore Instance => LazyInstance.Value;

    private const int MaxSessions = 1000;
    private static readonly TimeSpan SessionExpiry = TimeSpan.FromHours(24);

    private readonly ConcurrentDictionary<ulong, SessionEntry> _sessions = new();

    private class SessionEntry
    {
        public required string SessionId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime LastAccessedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// 新しいセッションを登録する
    /// </summary>
    /// <param name="botMessageId">botが返信したメッセージのID</param>
    /// <param name="sessionId">ClaudeのセッションID</param>
    public void RegisterSession(ulong botMessageId, string sessionId)
    {
        CleanupIfNeeded();

        var entry = new SessionEntry
        {
            SessionId = sessionId,
            CreatedAt = TimeManager.GetNow(),
            LastAccessedAt = TimeManager.GetNow()
        };

        _sessions[botMessageId] = entry;
    }

    /// <summary>
    /// DiscordメッセージIDからセッションIDを取得する
    /// </summary>
    /// <param name="referencedMessageId">参照されているbotのメッセージID</param>
    /// <returns>セッションID（見つからない場合やInactiveの場合はnull）</returns>
    public string? GetSessionId(ulong referencedMessageId)
    {
        if (!_sessions.TryGetValue(referencedMessageId, out var entry))
            return null;

        if (!entry.IsActive)
            return null;

        var now = TimeManager.GetNow();
        if (now - entry.CreatedAt > SessionExpiry)
        {
            _sessions.TryRemove(referencedMessageId, out _);
            return null;
        }

        entry.LastAccessedAt = now;
        return entry.SessionId;
    }


    /// <summary>
    /// セッションを無効化する（context-window limitに達した場合など）
    /// </summary>
    /// <param name="botMessageId">botが返信したメッセージのID</param>
    public void InvalidateSession(ulong botMessageId)
    {
        if (_sessions.TryGetValue(botMessageId, out var entry))
        {
            entry.IsActive = false;
        }
    }

    /// <summary>
    /// セッションが存在するか確認する
    /// </summary>
    public bool HasSession(ulong referencedMessageId)
    {
        if (!_sessions.TryGetValue(referencedMessageId, out var entry))
            return false;

        var now = TimeManager.GetNow();
        if (now - entry.CreatedAt > SessionExpiry)
        {
            _sessions.TryRemove(referencedMessageId, out _);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 必要に応じてクリーンアップを実行
    /// </summary>
    private void CleanupIfNeeded()
    {
        if (_sessions.Count < MaxSessions)
            return;

        var now = TimeManager.GetNow();
        var keysToRemove = _sessions
            .Where(kvp => now - kvp.Value.CreatedAt > SessionExpiry || !kvp.Value.IsActive)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _sessions.TryRemove(key, out _);
        }

        // まだ上限を超えている場合は古いものから削除
        if (_sessions.Count >= MaxSessions)
        {
            var oldestKeys = _sessions
                .OrderBy(kvp => kvp.Value.LastAccessedAt)
                .Take(_sessions.Count - MaxSessions + 100)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in oldestKeys)
            {
                _sessions.TryRemove(key, out _);
            }
        }
    }
}
