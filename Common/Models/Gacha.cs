using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Approvers.King.Common;

public class Gacha
{
    [Key] public Guid Id { get; set; }

    public int HitProbabilityPermillage { get; set; }

    /// <summary>
    /// 現在のメッセージに反応する確率
    /// </summary>
    [NotMapped]
    public Multiplier HitProbability
    {
        get => Multiplier.FromPermillage(HitProbabilityPermillage);
        set => HitProbabilityPermillage = value.Permillage;
    }

    /// <summary>
    /// 各メッセージの確率
    /// </summary>
    public List<GachaItem> GachaItems { get; set; } = [];

    /// <summary>
    /// 単発ガチャを回す
    /// </summary>
    public GachaItem? RollOnce()
    {
        if (RandomManager.IsHit(HitProbability) == false)
        {
            return null;
        }

        return GetRandomResult();
    }

    /// <summary>
    /// 確定ガチャを回す
    /// </summary>
    public GachaItem RollOnceCertain()
    {
        return GetRandomResult();
    }

    private GachaItem GetRandomResult()
    {
        var total = GachaItems.Sum(x => x.Probability.Permillage);
        if (total <= 0)
        {
            return GachaItems.First();
        }

        var value = RandomManager.GetRandomInt(total);

        foreach (var element in GachaItems)
        {
            if (value < element.Probability.Permillage) return element;
            value -= element.Probability.Permillage;
        }

        return GachaItems.Last();
    }

    /// <summary>
    /// メッセージに反応する確率を再抽選する
    /// </summary>
    public void ShuffleRareReplyRate()
    {
        // 確率は step の単位で max まで変動（ただし0にはならない）
        var step = MasterManager.SettingMaster.RareReplyProbabilityStep;
        var max = MasterManager.SettingMaster.MaxRareReplyProbability;
        var rates = Enumerable.Range(0, max.Permillage / step.Permillage).Select(i => step * (i + 1));
        HitProbability = RandomManager.PickRandom(rates);
    }

    /// <summary>
    /// 各メッセージの確率を再抽選する
    /// </summary>
    public void ShuffleMessageRates()
    {
        var items = MasterManager.RandomMessageMaster
            .GetAll(x => x.Type == RandomMessageType.GeneralReply)
            .Select(randomMessage => GachaItems.FirstOrDefault(item => item.RandomMessageId == randomMessage.Id) ?? new GachaItem()
            {
                GachaId = Id,
                RandomMessageId = randomMessage.Id,
                Probability = Multiplier.Zero
            })
            .ToList();

        // いい感じに確率がばらけるように、カイ二乗分布を適用
        var borders = Enumerable.Range(0, items.Count - 1)
            .Select(_ => (float)Math.Pow(RandomManager.GetRandomFloat(1f), 2))
            .Select(Multiplier.FromRate)
            .OrderBy(x => x)
            .ToList();
        borders.Add(Multiplier.One);
        var randomIndices = RandomManager.Shuffle(Enumerable.Range(0, items.Count)).ToList();

        items[randomIndices[0]].Probability = borders[0];
        for (var i = 1; i < items.Count; i++)
        {
            items[randomIndices[i]].Probability = borders[i] - borders[i - 1];
        }

        GachaItems.Clear();
        GachaItems.AddRange(items);
    }
}
