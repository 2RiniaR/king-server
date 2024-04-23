using System.Timers;
using Timer = System.Timers.Timer;

namespace Approvers.King.Common;

public static class SchedulerManager
{
    private static readonly Timer Timer = new(TimeSpan.FromSeconds(1));
    private static readonly List<SchedulerJobRunner> Runners = new();

    public static void Initialize()
    {
        Timer.Elapsed += OnEverySecond;
        Timer.Start();
    }

    private static void OnEverySecond(object? sender, ElapsedEventArgs e)
    {
        var now = TimeManager.GetNow();
        foreach (var runner in Runners)
            if (runner.Predicate != null && runner.Predicate(now))
                runner.Run();
    }

    public static void RegisterDaily<T>(TimeSpan time) where T : SchedulerJobPresenterBase, new()
    {
        Runners.Add(new SchedulerJobRunner<T>
        {
            Predicate = x => x.Hour == time.Hours &&
                             x.Minute == time.Minutes &&
                             x.Second == time.Seconds
        });
    }

    public static void RegisterYearly<T>(DateTime datetime) where T : SchedulerJobPresenterBase, new()
    {
        Runners.Add(new SchedulerJobRunner<T>
        {
            Predicate = x => x.Month == datetime.Month &&
                             x.Day == datetime.Day &&
                             x.Hour == datetime.Hour &&
                             x.Minute == datetime.Minute &&
                             x.Second == datetime.Second
        });
    }
}
