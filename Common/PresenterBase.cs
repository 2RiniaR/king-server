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
        catch (AppException e)
        {
            await SendAppError(e);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync("========================================\n");
            Console.Error.Write(e);
        }
    }

    /// <summary>
    /// アプリ内エラーが発生した時の処理
    /// </summary>
    protected abstract Task SendAppError(AppException e);

    /// <summary>
    /// メインで実行される処理
    /// </summary>
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
