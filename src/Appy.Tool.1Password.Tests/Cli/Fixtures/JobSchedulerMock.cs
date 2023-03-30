using System;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.Scheduling;
using Moq;

namespace Appy.Tool.OnePassword.Tests.Cli.Fixtures;

public class JobSchedulerMock : Mock<IJobScheduler>
{
    public void SetupAndExecuteJob()
    {
        Setup(x => x.ScheduleJobAndBlock(
                It.IsAny<Func<Task>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .Callback((Func<Task> func, TimeSpan interval, CancellationToken cancellationToken) =>
                func());
    }

    public void VerifyCalledWith(TimeSpan expected)
    {
        Verify(x => x.ScheduleJobAndBlock(
            It.IsAny<Func<Task>>(),
            It.Is<TimeSpan>(interval => interval == expected),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}