using System;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tests.Fixtures;
using Appy.Tool.OnePassword.Api;
using Xunit;
using Appy.Tool.OnePassword.Cli;
using Appy.Tool.OnePassword.Tests.Cli.Fixtures;
using Appy.Tool.OnePassword.Tests.Cli.Utils;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;

namespace Appy.Tool.OnePassword.Tests.Cli
{
    public static class OnePasswordToolCliTests
    {
        public class WhenSigninOptionIsMissing
        {
            [Fact]
            public async Task ShouldLogMissingOptionError()
            {
                var fixture = new Fixture();

                var sut = fixture.CreateSubject();

                var args = new[] { "-vt", "test" };

                var result = await sut.ExecuteAsync(args);

                result.Should().Be(1);
                fixture.Logger.VerifyLogError("Signin option must be specified.");
            }
        }

        public class WhenSigninOptionHasInvalidValuesCount
        {
            [Theory]
            [InlineData("-s test test")]
            [InlineData("-s test test test test")]
            public async Task ShouldLogInvalidValuesError(string values)
            {
                var fixture = new Fixture();

                var args = values.Split(' ');

                var sut = fixture.CreateSubject();

                var result = await sut.ExecuteAsync(args);

                result.Should().Be(1);
                fixture.Logger.VerifyLogError("Signin values must be specified: <organization> <email_address> <secret_key>.");
            }
        }

        public class WhenSigninOptionHasNoValues
        {
            [Fact]
            public async Task ShouldSigninWithCurrentSessionOrganization()
            {
                var fixture = new Fixture().WithValidCurrentSession();
                var expectedCmd = new SignInOnePasswordCommand {Organization = fixture.Organization};

                var sut = fixture.CreateSubject();
                var args = new[] { "-s" };

                _ = await sut.ExecuteAsync(args);

                fixture.Tool.VerifyCalledWith(expectedCmd);
            }
        }

        public class WhenSigninOptionHasAllValues
        {
            [Fact]
            public async Task ShouldSigninWithGivenValuesAndFirstSignIn()
            {
                var fixture = new Fixture().WithValidCurrentSession();
                var expectedCmd = new SignInOnePasswordCommand
                {
                    Organization = fixture.Organization,
                    Email = fixture.Email,
                    SecretKey = fixture.Secretkey,
                    IsFirstSignIn = true
                };

                var sut = fixture.CreateSubject();
                var args = new[] { "-s", fixture.Organization, fixture.Email, fixture.Secretkey };

                _ = await sut.ExecuteAsync(args);

                fixture.Tool.VerifyCalledWith(expectedCmd);
            }
        }

        public class WhenSessionOptionsNoSet
        {
            [Fact]
            public async Task ShouldUpdateSessionWithCurrentStoredValues()
            {
                var fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult();

                var expected = AppyOnePasswordSession.New(
                    organization: fixture.Organization,
                    vault: fixture.Vault,
                    environment: fixture.Environment,
                    sessionToken: fixture.SessionToken);

                var sut = fixture.CreateSubject();
                var args = new[] { "-s" };

                _ = await sut.ExecuteAsync(args);

                fixture.SessionStorage.VerifyUpdateWith(expected);
            }
        }

        public class WhenEnvironmentOptionHasValue
        {
            [Fact]
            public async Task ShouldUpdateSessionWithGivenEnvironment()
            {
                var fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult();
                var environment = "LIVE";
                var expected = AppyOnePasswordSession.New(
                    organization: fixture.Organization,
                    vault: fixture.Vault,
                    environment: environment,
                    sessionToken: fixture.SessionToken);

                var sut = fixture.CreateSubject();
                var args = new[] { "-s", "-env", environment};

                _ = await sut.ExecuteAsync(args);

                fixture.SessionStorage.VerifyUpdateWith(expected);
            }
        }

        public class WhenVaultOptionHasValue
        {
            [Fact]
            public async Task ShouldUpdateSessionWithGivenVault()
            {
                var fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult();

                var vault = "Private";
                var expected = AppyOnePasswordSession.New(
                    organization: fixture.Organization,
                    vault: vault,
                    environment: fixture.Environment,
                    sessionToken: fixture.SessionToken);

                var sut = fixture.CreateSubject();
                var args = new[] { "-s", "-vt", vault};

                _ = await sut.ExecuteAsync(args);

                fixture.SessionStorage.VerifyUpdateWith(expected);
            }
        }

        public class WhenSessionIsValid : IAsyncLifetime
        {
            readonly Fixture _fixture;

            public WhenSessionIsValid()
            {
                _fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult();
            }

            public async Task InitializeAsync()
            {
                var sut = _fixture.CreateSubject();
                var args = new[] { "-s"};

                _ = await sut.ExecuteAsync(args);
            }

            [Fact]
            public void ShouldRenderSessionWithConsoleVisualizer()
            {
                var expected = AppyOnePasswordSession.New(
                    organization: _fixture.Organization,
                    vault: _fixture.Vault,
                    environment: _fixture.Environment,
                    sessionToken: _fixture.SessionToken);

                _fixture.ConsoleVisualizer.VerifyCalledWith(expected);
            }

            [Fact]
            public void ShouldLogInfoToStartProjectDebugSession()
            {
                _fixture.Logger.VerifyLogInformation("You can now go to your project and start your debug session.");
            }

            public Task DisposeAsync() => Task.CompletedTask;
        }

        public class WhenAutoRenewOptionHasNotBeenSet
        {
            [Fact]
            public async Task ShouldLogOnePasswordExpirationMessage()
            {
                var fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult();

                var sut = fixture.CreateSubject();
                var args = new[] { "-s" };

                var result = await sut.ExecuteAsync(args);

                fixture.Logger.VerifyLogInformation("Session tokens expire after 30 minutes of inactivity, after which you’ll need to sign in again.");
                result.Should().Be(0);
            }
        }

        public class WhenApiOptionWithoutAutoRenewOption
        {
            [Fact]
            public async Task ShouldStartApiWithBlockingInDefaultPort()
            {
                var fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult();

                var expected = new OnePasswordApiSettings
                {
                    StartWithoutBlocking = false,
                    Port = 5500
                };

                var sut = fixture.CreateSubject();
                var args = new[] { "-s", "-api" };

                _ = await sut.ExecuteAsync(args);

                fixture.Logger.VerifyLogInformation($"Appy 1Password session API listening on port: {expected.Port}");
                fixture.ApiRunner.VerifyCalledWith(expected);
            }
        }

        public class WhenApiOptionWithAutoRenewOptionSet
        {
            [Fact]
            public async Task ShouldStartApiWithoutBlocking()
            {
                var fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult();

                var expected = new OnePasswordApiSettings
                {
                    StartWithoutBlocking = true,
                    Port = 5500
                };

                var sut = fixture.CreateSubject();
                var args = new[] { "-s", "-a", "-api"};

                _ = await sut.ExecuteAsync(args);

                fixture.ApiRunner.VerifyCalledWith(expected);
            }
        }

        public class WhenApiOptionAndPortHasBeenSet
        {
            [Fact]
            public async Task ShouldStartApiWithBlockingInGivenPort()
            {
                var fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult();

                var expected = new OnePasswordApiSettings
                {
                    StartWithoutBlocking = false,
                    Port = 6000
                };

                var sut = fixture.CreateSubject();
                var args = new[] { "-s", "-api", "6000" };

                _ = await sut.ExecuteAsync(args);

                fixture.Logger.VerifyLogInformation($"Appy 1Password session API listening on port: {expected.Port}");
                fixture.ApiRunner.VerifyCalledWith(expected);
            }
        }

        public class WhenAutoRenewOptionHasBeenSet : IAsyncLifetime
        {
            readonly Fixture _fixture;

            public WhenAutoRenewOptionHasBeenSet()
            {
                _fixture = new Fixture()
                    .WithValidCurrentSession()
                    .WithSuccessfulSignInResult()
                    .WithoutJobSchedulerDelay();
            }

            public async Task InitializeAsync()
            {
                var sut = _fixture.CreateSubject();
                var args = new[] { "-s", "-a" };

                _ = await sut.ExecuteAsync(args);
            }

            [Fact]
            public void ShouldScheduleJobWithOnePasswordExpirationInternal()
            {
                var expected = TimeSpan.FromMinutes(29);

                _fixture.JobScheduler.VerifyCalledWith(expected);
            }

            [Fact]
            public void ShouldLogInfoAboutRenewSessionActivity()
            {
                var autoRenewDelayInMins = 29;

                _fixture.Logger.VerifyLogInformation($"Self-renewing the session activity after {autoRenewDelayInMins} min.");

                _fixture.Logger.VerifyLogInformation($"Session activity automatically renewed after {autoRenewDelayInMins} min.");

                _fixture.Logger.VerifyLogInformation($"Your session activity will be auto renewed in {autoRenewDelayInMins} min.", times: 2);
            }

            [Fact]
            public void ShouldAutoRenewSessionActivityWithVaultsQuery()
            {
                var expected = new GetOnePasswordVaultsQuery
                {
                    Organization = _fixture.Organization,
                    SessionToken = _fixture.SessionToken
                };

                _fixture.Tool.VerifyCalledWith(expected);
            }

            public Task DisposeAsync() => Task.CompletedTask;
        }

        class Fixture
        {
            public string Organization { get; }
            public string Email { get; }
            public string Secretkey { get; }
            public string Environment { get; }
            public string Vault { get; }
            public string SessionToken { get; }
            public Fixture()
            {
                Logger = new LoggerMock();
                ConsoleVisualizer = new ConsoleVisualizerMock();
                ApiRunner = new OnePasswordApiRunnerMock();
                SessionStorage = new OnePasswordSessionStorageMock();
                JobScheduler = new JobSchedulerMock();
                Tool = new OnePasswordToolMock();
                CommandLineApplicationFactory = new CommandLineApplicationFactoryMock();

                var commandLineApp = new CommandLineApplication(new TestConsole(null));
                CommandLineApplicationFactory.SetupAndReturns(commandLineApp);

                Organization = "appy";
                Email = "test@appy.com";
                Secretkey = "FakeSecretKey";
                Environment = "DEV";
                Vault = "Development";
                SessionToken = "FakeToken";
            }

            public LoggerMock Logger { get; }
            public CommandLineApplicationFactoryMock CommandLineApplicationFactory { get; }
            public ConsoleVisualizerMock ConsoleVisualizer { get; }
            public OnePasswordApiRunnerMock ApiRunner { get; }
            public OnePasswordSessionStorageMock SessionStorage { get; }
            public JobSchedulerMock JobScheduler { get; }
            public OnePasswordToolMock Tool { get; }
            public IAppyOnePasswordToolCli CreateSubject()
            {
                return new AppyOnePasswordToolCli(
                    Logger.Object,
                    CommandLineApplicationFactory.Object,
                    ConsoleVisualizer.Object,
                    ApiRunner.Object,
                    SessionStorage.Object,
                    JobScheduler.Object,
                    Tool.Object);
            }

            public Fixture WithValidCurrentSession()
            {
                SessionStorage.SetupGetAndReturns(AppyOnePasswordSession.New(
                    organization: Organization,
                    vault: Vault,
                    environment: Environment,
                    sessionToken: SessionToken));
                return this;
            }

            public Fixture WithSuccessfulSignInResult()
            {
                Tool.SetupAndReturns(SignInOnePasswordResult.Create(SessionToken));
                return this;
            }

            public Fixture WithoutJobSchedulerDelay()
            {
                JobScheduler.SetupAndExecuteJob();
                return this;
            }
        }
    }
}