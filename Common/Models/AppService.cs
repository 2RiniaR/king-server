using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Common;

/// <summary>
/// DB上のデータモデル
/// </summary>
public class AppService : DbContext
{
    public DbSet<AppState> AppStates { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<GachaProbability> GachaProbabilities { get; set; }

    /// <summary>
    /// セッションを作成する
    /// dbを読み書きする際は、このメソッドを使う
    /// </summary>
    public static AppService CreateSession()
    {
        return new AppService();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(EnvironmentManager.SqliteConnectionString);
    }

    /// <summary>
    /// ユーザーを取得する なければ作成する
    /// </summary>
    public async Task<User> FindOrCreateUserAsync(ulong discordId)
    {
        var user = await Users.FindAsync(discordId);
        if (user != null)
        {
            return user;
        }

        user = new User { DiscordId = discordId };
        Add(user);
        return user;
    }
    
    public async Task<Slot> GetDefaultSlotAsync()
    {
        var slot = await Slots.FirstOrDefaultAsync();
        if (slot != null)
        {
            return slot;
        }

        slot = new Slot { Id = Guid.NewGuid() };
        Add(slot);
        return slot;
    }
}
