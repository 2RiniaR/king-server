using Approvers.King.Common;

namespace Approvers.King.Events;

public static class MessageUtility
{
    public static string PickRandomMessage()
    {
        var totalRate = MasterManager.ReplyMessages.Sum(x => x.rate);
        var value = RandomUtility.GetRandomFloat(totalRate);

        foreach (var (rate, message) in MasterManager.ReplyMessages)
        {
            if (value < rate) return message;

            value -= rate;
        }

        return MasterManager.ReplyMessages[^1].message;
    }
}
