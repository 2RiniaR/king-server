using System.Timers;
using Timer = System.Timers.Timer;

namespace Approvers.King.Common;

public class SchedulerTimer
{
    private readonly Timer _timer = new(TimeSpan.FromSeconds(1));
    private readonly List<SchedulerJobRunner> _runners = new();

    public void Initialize()
    {
        _timer.Elapsed += OnEverySecond;
        _timer.Start();
    }

    private void OnEverySecond(object? sender, ElapsedEventArgs e)
    {
        var now = TimeManager.GetNow();
        foreach (var runner in _runners)
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
    public void RegisterDaily<T>(TimeSpan time) where T : SchedulerJobPresenterBase, new()
    {
        _runners.Add(new SchedulerJobRunner<T>
        {
            Predicate = x => x.Hour == time.Hours &&
                             x.Minute == time.Minutes &&
                             x.Second == time.Seconds
        });
    }

    /// <summary>
    /// 毎月のイベントを登録する
    /// </summary>
    public void RegisterMonthly<T>(int day, TimeSpan time) where T : SchedulerJobPresenterBase, new()
    {
        _runners.Add(new SchedulerJobRunner<T>
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
    public void RegisterYearly<T>(DateTime datetime) where T : SchedulerJobPresenterBase, new()
    {
        _runners.Add(new SchedulerJobRunner<T>
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
    public void RegisterOn<T>(Predicate<DateTime> predicate) where T : SchedulerJobPresenterBase, new()
    {
        _runners.Add(new SchedulerJobRunner<T>
        {
            Predicate = predicate,
            OnRiseOnly = true
        });
    }
}
