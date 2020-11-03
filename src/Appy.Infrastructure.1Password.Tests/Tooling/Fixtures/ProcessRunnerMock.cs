using Appy.Configuration.IO;
using Appy.TestTools;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling.Fixtures
{
    public class ProcessRunnerMock: Mock<IProcessRunner>
    {
        public ProcessRunnerMock SetupAndReturns(ProcessResult processResult)
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

        public void VerifyRunWith(string expectedTool, ProcessSettings expectedSettings)
        {
            Verify(x => x.Run(
                It.Is<string>(tool => tool == expectedTool),
                It.Is<ProcessSettings>(ps => IsEquivalentTo(ps, expectedSettings))));
        }
    }
}