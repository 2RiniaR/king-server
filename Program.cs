using Approvers.King.Common;

namespace Approvers.King;

public class Program
{
    private static void Main(string[] args)
    {
        BuildAsync(args).GetAwaiter().GetResult();
    }

    private static async Task BuildAsync(string[] args)
    {
        await DiscordManager.InitializeAsync();

        DiscordEntry.RegisterEvents();

        // 永久に待つ
        await Task.Delay(-1);
    }
}