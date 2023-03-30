using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appy.Configuration.Scheduling;

public interface IJobScheduler
{
    Task ScheduleJobAndBlock(Func<Task> job, TimeSpan interval, CancellationToken cancellationToken);
}