using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appy.Configuration.Common;

public static class TaskCancellationExtensions
{
    /// <summary>
    /// add cancellation functionality to Task T with exception message
    /// </summary>
    /// <param name="task"></param>
    /// <param name="milliseconds"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public static async Task<T> CancelAfter<T>(this Task<T> task, int milliseconds)
    {
        var cts = new CancellationTokenSource();

        cts.CancelAfter(milliseconds);

        var tcs = new TaskCompletionSource<bool>();

        using (cts.Token.Register(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                throw new OperationCanceledException(cts.Token);
        }

        return await task.ConfigureAwait(false);
    }

    /// <summary>
    /// add cancellation functionality to Task
    /// </summary>
    /// <param name="task"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public static async Task CancelAfter(this Task task, int milliseconds)
    {
        var cts = new CancellationTokenSource();

        cts.CancelAfter(milliseconds);

        var tcs = new TaskCompletionSource<bool>();

        using (cts.Token.Register(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                throw new OperationCanceledException(cts.Token);
        }

        await task.ConfigureAwait(false);
    }
}