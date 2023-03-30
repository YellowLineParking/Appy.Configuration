using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appy.Configuration.Scheduling;

public class SimpleJobScheduler : IJobScheduler
{
    public async Task ScheduleJobAndBlock(Func<Task> job, TimeSpan interval, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(interval, cancellationToken).ConfigureAwait(false);

            await job().ConfigureAwait(false);
        }
    }
}