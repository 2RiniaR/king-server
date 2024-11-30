using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Common;

public class GachaManager : Singleton<GachaManager>
{
    private readonly List<GachaProbability> _replyMessageTable = new();

    /// <summary>
    /// 現在のメッセージに反応する確率
    /// </summary>
    public float RareReplyRate { get; private set; }

    /// <summary>
    /// 各メッセージの確率
    /// </summary>
    public IReadOnlyList<GachaProbability> ReplyMessageTable => _replyMessageTable;

    public bool IsTableEmpty => ReplyMessageTable.Count == 0;

    /// <summary>
    /// 現在の状態を読み込む
    /// </summary>
    public async Task LoadAsync()
    {
        await using var app = AppService.CreateSession();

        var probabilities = await app.GachaProbabilities.ToListAsync();
        _replyMessageTable.Clear();
        _replyMessageTable.AddRange(probabilities.Where(x => x.RandomMessage != null));

        var rareReplyRatePermillage = await app.AppStates.GetIntAsync(AppStateType.RareReplyProbabilityPermillage);
        RareReplyRate = NumberUtility.GetPercentFromPermillage(rareReplyRatePermillage ?? 0);
    }

    /// <summary>
    /// 現在の状態を保存する
    /// </summary>
    public async Task SaveAsync()
    {
        await using var app = AppService.CreateSession();

        app.GachaProbabilities.RemoveRange(app.GachaProbabilities);
        app.GachaProbabilities.AddRange(_replyMessageTable);

        var rareReplyRatePermillage = NumberUtility.GetPermillageFromPercent(RareReplyRate);
        await app.AppStates.SetIntAsync(AppStateType.RareReplyProbabilityPermillage, rareReplyRatePermillage);

        await app.SaveChangesAsync();
    }

    /// <summary>
    /// マスタデータを読み込む
    /// </summary>
    public void LoadMaster()
    {
        // 同じIDのメッセージは確率を保持し、新規追加分は確率0%で初期化する
        var messages = MasterManager.RandomMessageMaster
            .GetAll(x => x.Type == RandomMessageType.GeneralReply)
            .Select(x => new GachaProbability()
            {
                RandomMessageId = x.Id,
                Probability = _replyMessageTable.FirstOrDefault(m => m.RandomMessageId == x.Id)?.Probability ?? 0
            })
            .ToList();

        _replyMessageTable.Clear();
        _replyMessageTable.AddRange(messages);
    }

    /// <summary>
    /// 単発ガチャを回す
    /// </summary>
    public GachaProbability? Roll()
    {
        if (RandomManager.IsHit(RareReplyRate) == false) return null;
        return GetRandomResult();
    }

    /// <summary>
    /// 確定ガチャを回す
    /// </summary>
    public GachaProbability RollWithoutNone()
    {
        return GetRandomResult();
    }

    private GachaProbability GetRandomResult()
    {
        var totalRate = _replyMessageTable.Sum(x => x.Probability);
        if (totalRate <= 0)
        {
            return _replyMessageTable[0];
        }

        var value = RandomManager.GetRandomFloat(totalRate);

        foreach (var element in _replyMessageTable)
        {
            if (value < element.Probability) return element;
            value -= element.Probability;
        }

        return _replyMessageTable[^1];
    }

    /// <summary>
    /// メッセージに反応する確率を再抽選する
    /// </summary>
    public void ShuffleRareReplyRate()
    {
        // 確率は step の単位で max まで変動（ただし0にはならない）
        var step = MasterManager.SettingMaster.RareReplyProbabilityStepPermillage;
        var max = MasterManager.SettingMaster.MaxRareReplyProbabilityPermillage;
        var rates = Enumerable.Range(0, max / step)
            .Select(i => NumberUtility.GetPercentFromPermillage((i + 1) * step));
        RareReplyRate = RandomManager.PickRandom(rates);
    }

    /// <summary>
    /// 各メッセージの確率を再抽選する
    /// </summary>
    public void ShuffleMessageRates()
    {
        // いい感じに確率がばらけるように、カイ二乗分布を適用
        var borders = Enumerable.Range(0, _replyMessageTable.Count - 1)
            .Select(x => (float)Math.Pow(RandomManager.GetRandomFloat(1f), 2))
            .Select(x => (int)Math.Floor(x * 100f))
            .OrderBy(x => x)
            .ToList();
        borders.Add(100);
        var randomIndices = RandomManager.Shuffle(Enumerable.Range(0, _replyMessageTable.Count)).ToList();

        _replyMessageTable[randomIndices[0]].Probability = borders[0] * 0.01f;
        for (var i = 1; i < _replyMessageTable.Count; i++)
        {
            _replyMessageTable[randomIndices[i]].Probability = (borders[i] - borders[i - 1]) * 0.01f;
        }
    }
}
