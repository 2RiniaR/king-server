namespace Approvers.King.Common;

public static class MasterManager
{
    public static IReadOnlyList<float> RareReplyRateTable => new List<float>
    {
        0.01f, 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.10f,
    };

    public static float ReplyMaxDelay => 2f;
    public static float TypingMaxDelay => 1f;

    public static IReadOnlyList<string> ReplyMessages => new List<string>
    {
        "あほしね", "ばか", "かす", "わかる", "草", "あほくさ", "noob", "それね、ちょっと分かる",
    };

    public static string SilentCommandReplyMessage => "（いっそうの気配が{0}から消え去った）";

    public static IReadOnlyList<string> SilentTriggerMessages => new List<string>
    {
        "だまれ", "黙れ", "だまって", "黙って", "だまろう", "黙ろう", "damare",
        "しゃべるな", "喋るな", "しゃべんな", "喋んな",
        "帰れ", "かえれ", "帰って", "かえって", "帰ろう", "かえろう", "kaere",
    };

    public static TimeSpan SilentTimeSpan => TimeSpan.FromMinutes(5);

    public static IReadOnlyList<string> GachaTriggerMessages => new List<string>
    {
        "10連", "ガチャ", "おみくじ",
    };

    public static IReadOnlyList<string> RidiculeMessages => new List<string>
    {
        "雑魚", "雑魚じゃん", "雑魚すぎ",
    };

    public static TimeSpan DailyResetTime => TimeSpan.FromHours(0);
}