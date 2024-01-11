using Discord;

namespace Approvers.King.Common;

public abstract class PresenterBase
{
    public async Task RunAsync()
    {
        try
        {
            await MainAsync();
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync("========================================\n");
            Console.Error.Write(e);
        }
    }

    protected abstract Task MainAsync();

    public static Embed ErrorEmbed(string message)
    {
        return new EmbedBuilder()
            .WithColor(Color.Red)
            .WithTitle("⚠ エラー")
            .WithDescription(message)
            .WithCurrentTimestamp()
            .Build();
    }
}