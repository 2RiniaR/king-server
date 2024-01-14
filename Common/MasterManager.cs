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

    public static string SilentCommandReplyMessage => "（いっそうの気配が{0}から消え去った）";

    public static IReadOnlyList<string> SilentTriggerMessages => new List<string>
    {
        "だまれ",
        "黙れ",
        "だまって",
        "黙って",
        "だまろう",
        "黙ろう",
        "damare",
        "しゃべるな",
        "喋るな",
        "しゃべんな",
        "喋んな",
        "帰れ",
        "かえれ",
        "帰って",
        "かえって",
        "帰ろう",
        "かえろう",
        "kaere",
    };

    public static TimeSpan SilentTimeSpan => TimeSpan.FromMinutes(5);

    public static IReadOnlyList<string> GachaTriggerMessages => new List<string>
    {
        "10連",
        "ガチャ",
        "おみくじ",
    };
}