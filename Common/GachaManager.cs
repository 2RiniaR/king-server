using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Common;

public class GachaManager : Singleton<GachaManager>
{
    private readonly List<ReplyMessage> _replyMessageTable = new();

    /// <summary>
    ///     現在のメッセージに反応する確率
    /// </summary>
    public float RareReplyRate { get; private set; }

    /// <summary>
    ///     各メッセージの確率
    /// </summary>
    public IReadOnlyList<ReplyMessage> ReplyMessageTable => _replyMessageTable;

    public bool IsTableEmpty => ReplyMessageTable.Count == 0;

    public async Task LoadAsync()
    {
        await using var app = AppService.CreateSession();

        var probabilities = await app.GachaProbabilities.ToListAsync();
        _replyMessageTable.Clear();
        foreach (var probability in probabilities)
        {
            var message = MasterManager.RandomMessageMaster.Find(probability.RandomMessageId);
            if (message == null) continue;
            _replyMessageTable.Add(new ReplyMessage
            {
                Rate = probability.Probability,
                Message = message
            });
        }

        var rareReplyRatePermillage = await app.AppStates.GetIntAsync(AppStateType.RareReplyProbabilityPermillage);
        RareReplyRate = NumberUtility.GetPercentFromPermillage(rareReplyRatePermillage ?? 0);
    }

    public async Task SaveAsync()
    {
        await using var app = AppService.CreateSession();

        app.GachaProbabilities.RemoveRange(app.GachaProbabilities);
        app.GachaProbabilities.AddRange(_replyMessageTable.Select(x => new GachaProbability
        {
            RandomMessageId = x.Message.Id,
            Probability = x.Rate
        }));

        var rareReplyRatePermillage = NumberUtility.GetPermillageFromPercent(RareReplyRate);
        await app.AppStates.SetIntAsync(AppStateType.RareReplyProbabilityPermillage, rareReplyRatePermillage);

        await app.SaveChangesAsync();
    }

    public void RefreshMessageTable()
    {
        _replyMessageTable.Clear();
        var messages = MasterManager.RandomMessageMaster
            .GetAll(x => x.Type == RandomMessageType.GeneralReply)
            .Select(x => new ReplyMessage { Rate = 1f, Message = x });
        _replyMessageTable.AddRange(messages);
    }

    public RandomMessage? Roll()
    {
        if (RandomUtility.IsHit(RareReplyRate) == false) return null;
        return GetRandomResult();
    }

    public RandomMessage RollWithoutNone()
    {
        return GetRandomResult();
    }

    private RandomMessage GetRandomResult()
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
        var step = MasterManager.SettingMaster.RareReplyProbabilityStepPermillage;
        var max = MasterManager.SettingMaster.MaxRareReplyProbabilityPermillage;
        RareReplyRate = Enumerable.Range(0, max / step)
            .Select(i => NumberUtility.GetPercentFromPermillage((i + 1) * step))
            .PickRandom();
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
        public RandomMessage Message { get; set; }
    }
}
