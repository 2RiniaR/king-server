using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Common;

[PrimaryKey(nameof(GachaId), nameof(RandomMessageId))]
public class GachaItem
{
    public Guid GachaId { get; set; }
    public Gacha Gacha { get; set; } = null!;
    public string RandomMessageId { get; set; } = null!;

    public int ProbabilityPermillage { get; set; }

    [NotMapped]
    public Multiplier Probability
    {
        get => Multiplier.FromPermillage(ProbabilityPermillage);
        set => ProbabilityPermillage = value.Permillage;
    }

    private IssoRandomMessage? _randomMessage;

    public IssoRandomMessage? RandomMessage => _randomMessage == null || _randomMessage.Id != RandomMessageId
        ? _randomMessage = MasterManager.IssoRandomMessageMaster.Find(RandomMessageId)
        : _randomMessage;
}
