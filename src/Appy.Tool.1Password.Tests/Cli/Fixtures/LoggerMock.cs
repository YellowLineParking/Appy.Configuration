using Appy.Configuration.Logging;
using Moq;

namespace Appy.Tool.OnePassword.Tests.Cli.Fixtures
{
    public class LoggerMock : Mock<ILogger>
    {
        public void VerifyLogInformation(string message, int times = 1)
        {
            Verify(x => x.LogInformation(
                It.Is<string>(msg => msg == message),
                It.IsAny<bool>()), Times.Exactly(times));
        }

        public void VerifyLogError(string message)
        {
            Verify(x => x.LogError(
                It.Is<string>(msg => msg == message)));
        }
    }
}