using System.ComponentModel.DataAnnotations;

namespace Approvers.King.Common;

public class GachaProbability
{
    [Key] public string RandomMessageId { get; set; }
    public float Probability { get; set; }
}
