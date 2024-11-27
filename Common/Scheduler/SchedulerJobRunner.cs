namespace Approvers.King.Common;

public abstract class SchedulerJobRunner
{
    public bool OnRiseOnly { get; set; }
    public bool? PreviousCondition = null;
    public Predicate<DateTime>? Predicate { get; init; }
    public abstract void Run();
}

public class SchedulerJobRunner<T> : SchedulerJobRunner where T : SchedulerJobPresenterBase, new()
{
    public override void Run()
    {
        new T().RunAsync().Run();
    }
}
