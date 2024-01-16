using Approvers.King.Common;
using Approvers.King.Triggers;

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

        DiscordTrigger.RegisterEvents();

        // 永久に待つ
        await Task.Delay(-1);
    }
}
