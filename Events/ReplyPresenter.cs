using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class ReplyPresenter : DiscordMessagePresenterBase
{
    private static readonly Random Random = new();

    protected override async Task MainAsync()
    {
        var rand = Random.NextDouble();
        Console.WriteLine($"rand: {rand}");
        if (rand > MasterManager.ReplyRate) return;

        await Message.ReplyAsync(PickRandomMessage());
    }

    private static string PickRandomMessage()
    {
        var totalRate = MasterManager.ReplyMessages.Sum(x => x.rate);
        var value = Random.NextDouble() * totalRate;

        foreach (var (rate, message) in MasterManager.ReplyMessages)
        {
            if (value < rate) return message;

            value -= rate;
        }

        return MasterManager.ReplyMessages[^1].message;
    }
}