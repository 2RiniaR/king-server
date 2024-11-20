using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Common;

public class AppService : DbContext
{
    public DbSet<AppState> AppStates { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<GachaProbability> GachaProbabilities { get; set; }

    public static AppService CreateSession()
    {
        return new AppService();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(EnvironmentManager.SqliteConnectionString);
    }

    public async Task<User> FindOrCreateUserAsync(ulong discordId)
    {
        var user = await Users.FindAsync(discordId);
        if (user != null)
        {
            return user;
        }

        user = new User { DiscordID = discordId };
        Add(user);
        return user;
    }
}
