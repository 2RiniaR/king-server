using System.ComponentModel.DataAnnotations;

namespace Approvers.King.Common;

public class GachaProbability
{
    [Key] public string RandomMessageId { get; set; }
    public float Probability { get; set; }

    private RandomMessage? _randomMessage;

    public RandomMessage? RandomMessage => _randomMessage == null || _randomMessage.Id != RandomMessageId
        ? _randomMessage = MasterManager.RandomMessageMaster.Find(RandomMessageId)
        : _randomMessage;
}
