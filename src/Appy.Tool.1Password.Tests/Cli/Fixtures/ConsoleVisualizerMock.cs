using Appy.Infrastructure.OnePassword.Model;
using Appy.TestTools;
using Appy.Tool.OnePassword.Logging;
using Moq;

namespace Appy.Tool.OnePassword.Tests.Cli.Fixtures
{
    public class ConsoleVisualizerMock : Mock<IConsoleVisualzer>
    {
        public void VerifyCalledWith(AppyOnePasswordSession expected)
        {
            Verify(x => x.Render(
                It.Is<AppyOnePasswordSession>(session =>
                    session.IsEquivalentTo(expected))));
        }
    }
}