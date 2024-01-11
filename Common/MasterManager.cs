namespace Approvers.King.Common;

public static class MasterManager
{
    public static float ReplyRate => 0.05f;
    public static float ReplyMaxDelay => 2f;
    public static float TypingMaxDelay => 1f;

    public static IReadOnlyList<(float rate, string message)> ReplyMessages => new List<(float rate, string message)>
    {
        (1f, "あほしね"),
        (1f, "ばか"),
        (1f, "かす"),
        (1f, "わかる"),
        (1f, "草"),
        (1f, "あほくさ"),
        (1f, "noob"),
        (1f, "それね、ちょっと分かる"),
    };
}