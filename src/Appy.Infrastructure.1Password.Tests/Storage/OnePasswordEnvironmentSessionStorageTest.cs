using System;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.Infrastructure.OnePassword.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Storage
{
    public class OnePasswordEnvironmentSessionStorageTest
    {
        public class GetCurrent
        {
            public class WhenRunningOnWindows
            {
                [Fact]
                public async Task ShouldGetSessionInfoFromUserEnvironmentVariables()
                {
                    var fixture = new Fixture()
                        .WithWindowsPlatform();

                    var sut = fixture.CreateSubject();

                    var result = await sut.GetCurrent();

                    fixture.EnvAccessor.VerifyGetUserEnvCalledWith("APPY_OP_ORGANIZATION");
                    fixture.EnvAccessor.VerifyGetUserEnvCalledWith("APPY_OP_ENVIRONMENT");
                    fixture.EnvAccessor.VerifyGetUserEnvCalledWith("APPY_OP_SESSION_TOKEN");
                    fixture.EnvAccessor.VerifyGetUserEnvCalledWith("APPY_OP_VAULT");
                }
            }

            public class WhenNotRunningOnWindows
            {
                [Fact]
                public async Task ShouldGetSessionInfoFromProcessEnvironmentVariables()
                {
                    var fixture = new Fixture()
                        .WithoutWindowsPlatform();

                    var sut = fixture.CreateSubject();

                    var result = await sut.GetCurrent();

                    fixture.EnvAccessor.VerifyGetProcessEnvCalledWith("APPY_OP_ORGANIZATION");
                    fixture.EnvAccessor.VerifyGetProcessEnvCalledWith("APPY_OP_ENVIRONMENT");
                    fixture.EnvAccessor.VerifyGetProcessEnvCalledWith("APPY_OP_VAULT");
                    fixture.EnvAccessor.VerifyGetProcessEnvCalledWith("APPY_OP_SESSION_TOKEN");
                }
            }
        }

        public class UpdateSession
        {
            public class WhenNotRunningOnWindows
            {
                [Fact]
                public async Task ShouldThrowPlatformNotSupportedException()
                {
                    var fixture = new Fixture()
                        .WithoutWindowsPlatform();

                    var sut = fixture.CreateSubject();

                    var session = AppyOnePasswordSession.Empty();
                    Func<Task> act = () => sut.Update(session);

                    await act.Should().ThrowAsync<PlatformNotSupportedException>();
                }
            }

            public class WhenRunningOnWindowsAndInvalidSession
            {
                [Theory]
                [InlineData(null, null, null, null)]
                [InlineData("appy", null, null, null)]
                [InlineData("appy", "DEV", null, null)]
                [InlineData("appy", "DEV", "Development", null)]
                public async Task ShouldThrowArgumentException(string organization, string environment, string vault, string sessionToken)
                {
                    var fixture = new Fixture()
                        .WithWindowsPlatform();

                    var sut = fixture.CreateSubject();

                    var session = AppyOnePasswordSession.New(
                        organization: organization,
                        environment: environment,
                        vault: vault,
                        sessionToken: sessionToken);

                    Func<Task> act = () => sut.Update(session);

                    await act.Should().ThrowAsync<ArgumentException>();
                }
            }

            public class WhenRunningOnWindowsAndValidSession
            {
                [Fact]
                public async Task ShouldSetUserEnvironmentVariablesWithSessionInfo()
                {
                    var fixture = new Fixture()
                        .WithWindowsPlatform();

                    var sut = fixture.CreateSubject();

                    var session = AppyOnePasswordSession.New(
                        organization: fixture.Organization,
                        environment: fixture.Environment,
                        vault: fixture.Vault,
                        sessionToken: fixture.SessionToken);

                    await sut.Update(session);

                    fixture.EnvAccessor.VerifySetUserEnvCalledWith("APPY_OP_ORGANIZATION", session.Organization);
                    fixture.EnvAccessor.VerifySetUserEnvCalledWith("APPY_OP_ENVIRONMENT", session.Environment);
                    fixture.EnvAccessor.VerifySetUserEnvCalledWith("APPY_OP_VAULT", session.Vault);
                    fixture.EnvAccessor.VerifySetUserEnvCalledWith("APPY_OP_SESSION_TOKEN", session.SessionToken);
                }
            }
        }

        public class Fixture
        {
            public string Organization { get; }
            public string Environment { get; }
            public string Vault { get; }
            public string SessionToken { get; }

            public Fixture()
            {
                PlatformInfo = new PlatformInformationMock();
                EnvAccessor = new EnvironmentAccessorMock();
                Organization = "appy";
                Environment = "DEV";
                Vault = "Development";
                SessionToken = "FakeToken";
            }

            public PlatformInformationMock PlatformInfo { get; }
            public EnvironmentAccessorMock EnvAccessor { get; }

            public IOnePasswordSessionStorage CreateSubject()
            {
                return new OnePasswordEnvironmentSessionStorage(PlatformInfo.Object, EnvAccessor.Object);
            }

            public Fixture WithWindowsPlatform()
            {
                PlatformInfo.SetupRunningOnWindows(true);
                return this;
            }

            public Fixture WithoutWindowsPlatform()
            {
                PlatformInfo.SetupRunningOnWindows(false);
                return this;
            }
        }
    }
}