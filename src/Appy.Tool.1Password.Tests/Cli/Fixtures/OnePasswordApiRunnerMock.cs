using Appy.TestTools;
using Appy.Tool.OnePassword.Api;
using Moq;

namespace Appy.Tool.OnePassword.Tests.Cli.Fixtures;

public class OnePasswordApiRunnerMock : Mock<IOnePasswordApiRunner>
{
    public void VerifyCalledWith(OnePasswordApiSettings expected)
    {
        Verify(x => x.Start(
            It.Is<OnePasswordApiSettings>(settings =>
                settings.IsEquivalentTo(expected))));
    }
}