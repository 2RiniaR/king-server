namespace Approvers.King.Common;

public static class TaskUtility
{
    public static Task WaitAsync(Action<Func<Task>> register, Action<Func<Task>> unregister,
        CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource();
        Func<Task>? h = null;
        h = () =>
        {
            unregister(h!);
            tcs.SetResult();
            return Task.CompletedTask;
        };
        register(h);
        ct.Register(() =>
        {
            unregister(h);
            tcs.SetCanceled();
        });

        try
        {
            return tcs.Task;
        }
        catch
        {
            unregister(h);
            throw;
        }
    }
}
