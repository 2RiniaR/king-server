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
        {
            if (runner.Predicate != null && runner.Predicate(now)) runner.Run();
        }
    }

    public static void RegisterDaily<T>(TimeSpan time) where T : SchedulerJobPresenterBase, new()
    {
        Runners.Add(new SchedulerJobRunner<T>
        {
            Predicate = dateTime => dateTime.Hour == time.Hours &&
                                    dateTime.Minute == time.Minutes &&
                                    dateTime.Second == time.Seconds,
        });
    }
}