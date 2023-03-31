using Appy.Configuration.IO;
using Appy.TestTools;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Fixtures;

public class ProcessRunnerMock: Mock<IProcessRunner>
{
    public ProcessRunnerMock SetupRunOpAndReturns(ProcessSettings settings, ProcessResult processResult)
    {
        Setup(x => x.Run(
                It.Is<string>(toolPath => toolPath == "op"),
                It.Is<ProcessSettings>(ps => IsEquivalentTo(ps, settings))))
            .ReturnsAsync(processResult);
        return this;
    }

    public ProcessRunnerMock SetupRunAndReturns(ProcessResult processResult)
    {
        Setup(x => x.Run(
                It.IsAny<string>(),
                It.IsAny<ProcessSettings>()))
            .ReturnsAsync(processResult);
        return this;
    }

    static bool IsEquivalentTo(ProcessSettings source, ProcessSettings expected)
    {
        return source.Arguments.IsEquivalentTo(expected.Arguments) &&
               source.EnvironmentVariables.IsEquivalentTo(expected.EnvironmentVariables) &&
               source.RedirectStandardInput == expected.RedirectStandardInput &&
               source.CreateNoWindow == expected.CreateNoWindow;
    }

    public void VerifyRunCalledWith(string expectedTool, ProcessSettings expectedSettings, int times = 1)
    {
        Verify(x => x.Run(
            It.Is<string>(tool => tool == expectedTool),
            It.Is<ProcessSettings>(ps => IsEquivalentTo(ps, expectedSettings))), Times.Exactly(times));
    }

    public void VerifyRunNotCalledWith(string expectedTool, ProcessSettings expectedSettings)
    {
        VerifyRunCalledWith(expectedTool, expectedSettings, times: 0);
    }
}