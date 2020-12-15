using System;
using Appy.Configuration.Common;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Fixtures
{
    public class EnvironmentAccessorMock : Mock<IEnvironmentAccessor>
    {
        public void VerifyGetUserEnvCalledWith(string expected)
        {
            Verify(x => x.GetUserEnvironmentVariable(
                    It.Is<string>(env => env.Equals(expected, StringComparison.OrdinalIgnoreCase))),
                Times.Once);
        }

        public void VerifyGetProcessEnvCalledWith(string expected)
        {
            Verify(x => x.GetProcessEnvironmentVariable(
                    It.Is<string>(env => env.Equals(expected, StringComparison.OrdinalIgnoreCase))),
                Times.Once);
        }

        public void VerifySetUserEnvCalledWith(string expectedEnvVar, string expectedValue)
        {
            Verify(x => x.SetUserEnvironmentVariable(
                    It.Is<string>(env => env.Equals(expectedEnvVar, StringComparison.OrdinalIgnoreCase)),
                    It.Is<string>(value => value.Equals(expectedValue, StringComparison.OrdinalIgnoreCase))),
                Times.Once);
        }
    }
}