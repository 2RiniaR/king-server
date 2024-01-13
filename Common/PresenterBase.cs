using Discord;
using Discord.WebSocket;

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

    public static class Format
    {
        private static readonly string[] SanitizeTarget = { "\\", "*", "_", "~", "`", ".", ":", "/", ">", "|", "#" };

        public static string Sanitize(string text)
        {
            foreach (var target in SanitizeTarget)
            {
                text = text.Replace(target, "\\" + target);
            }

            return text;
        }

        public static string UserName(IUser user)
        {
            return Sanitize((user as SocketGuildUser)?.Nickname ?? user.Username);
        }
    }
}