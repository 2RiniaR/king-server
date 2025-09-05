namespace Approvers.King.Common;

public class AppCache : Singleton<AppCache>
{
    public DateTime? YouLastSendTime { get; set; }
}
