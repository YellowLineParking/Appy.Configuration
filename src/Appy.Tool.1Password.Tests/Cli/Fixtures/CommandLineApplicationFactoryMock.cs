using Appy.Tool.OnePassword.Cli;
using McMaster.Extensions.CommandLineUtils;
using Moq;

namespace Appy.Tool.OnePassword.Tests.Cli.Fixtures;

public class CommandLineApplicationFactoryMock : Mock<ICommandLineApplicationFactory>
{
    public CommandLineApplicationFactoryMock SetupAndReturns(CommandLineApplication app)
    {
        Setup(x => x.Create(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(app);
        return this;
    }
}