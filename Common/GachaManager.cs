namespace Approvers.King.Common;

public class GachaManager
{
    private readonly List<ReplyMessage> _replyMessageTable = new();

    public static GachaManager Instance { get; } = new();

    /// <summary>
    /// 現在のメッセージに反応する確率
    /// </summary>
    public float RareReplyRate { get; private set; }

    /// <summary>
    /// 各メッセージの確率
    /// </summary>
    public IReadOnlyList<ReplyMessage> ReplyMessageTable => _replyMessageTable;

    public void Initialize()
    {
        _replyMessageTable.Clear();
        _replyMessageTable.AddRange(MasterManager.ReplyMessages.Select(x => new ReplyMessage
        {
            Rate = 1f,
            Message = x,
        }));
    }

    public string? TryPickRareReplyMessage()
    {
        if (RandomUtility.IsHit(RareReplyRate) == false) return null;
        return PickMessage();
    }

    public string PickMessage()
    {
        var totalRate = _replyMessageTable.Sum(x => x.Rate);
        var value = RandomUtility.GetRandomFloat(totalRate);

        foreach (var element in _replyMessageTable)
        {
            if (value < element.Rate) return element.Message;
            value -= element.Rate;
        }

        return _replyMessageTable[^1].Message;
    }

    public void ShuffleRareReplyRate()
    {
        RareReplyRate = MasterManager.RareReplyRateTable.PickRandom();
    }

    public void ShuffleMessageRates()
    {
        var borders = Enumerable.Range(0, _replyMessageTable.Count - 1)
            .Select(x => (float)Math.Pow(RandomUtility.GetRandomFloat(1f), 2))
            .Select(x => (int)Math.Floor(x * 100f))
            .OrderBy(x => x)
            .ToList();
        borders.Add(100);
        var randomIndices = Enumerable.Range(0, _replyMessageTable.Count).Shuffle().ToList();

        _replyMessageTable[randomIndices[0]].Rate = borders[0] * 0.01f;
        for (var i = 1; i < _replyMessageTable.Count; i++)
        {
            _replyMessageTable[randomIndices[i]].Rate = (borders[i] - borders[i - 1]) * 0.01f;
        }
    }

    public class ReplyMessage
    {
        public float Rate { get; set; }
        public string Message { get; init; } = string.Empty;
    }
}