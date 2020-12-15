using Appy.Configuration.Common;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Fixtures
{
    public class PlatformInformationMock : Mock<IPlatformInformation>
    {
        public void SetupRunningOnWindows(bool value)
        {
            Setup(x => x.IsRunningOnWindows())
                .Returns(value);
        }
    }
}