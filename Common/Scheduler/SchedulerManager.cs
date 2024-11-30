using System.Timers;
using Timer = System.Timers.Timer;

namespace Approvers.King.Common;

/// <summary>
/// 時間で実行されるイベントを管理する
/// </summary>
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
            var condition = runner.Predicate != null && runner.Predicate(now);

            if (runner.OnRiseOnly == false)
            {
                if (condition)
                {
                    runner.Run();
                }
            }
            else
            {
                if (runner.PreviousCondition is false && condition)
                {
                    runner.Run();
                }
            }

            runner.PreviousCondition = condition;
        }
    }

    /// <summary>
    /// 毎日のイベントを登録する
    /// </summary>
    public static void RegisterDaily<T>(TimeSpan time) where T : SchedulerJobPresenterBase, new()
    {
        Runners.Add(new SchedulerJobRunner<T>
        {
            Predicate = x => x.Hour == time.Hours &&
                             x.Minute == time.Minutes &&
                             x.Second == time.Seconds
        });
    }

    /// <summary>
    /// 毎月のイベントを登録する
    /// </summary>
    public static void RegisterMonthly<T>(int day, TimeSpan time) where T : SchedulerJobPresenterBase, new()
    {
        Runners.Add(new SchedulerJobRunner<T>
        {
            Predicate = x => x.Day == day &&
                             x.Hour == time.Hours &&
                             x.Minute == time.Minutes &&
                             x.Second == time.Seconds
        });
    }

    /// <summary>
    /// 毎年のイベントを登録する
    /// </summary>
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

    /// <summary>
    /// 条件を満たしたタイミングで実行するイベントを登録する
    /// </summary>
    public static void RegisterOn<T>(Predicate<DateTime> predicate) where T : SchedulerJobPresenterBase, new()
    {
        Runners.Add(new SchedulerJobRunner<T>
        {
            Predicate = predicate,
            OnRiseOnly = true
        });
    }
}
