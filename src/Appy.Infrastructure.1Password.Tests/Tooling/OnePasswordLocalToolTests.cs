using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appy.Configuration.Common;
using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tests.Fixtures;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Infrastructure.OnePassword.Tooling.Internal;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling;

public class OnePasswordLocalToolTests
{
    public class FetchOnePasswordNoteQueryTests
    {
        public class WhenExecuteValidQuery : IAsyncLifetime
        {
            readonly Fixture _fixture;
            FetchOnePasswordNoteQuery _query;
            FetchOnePasswordNoteQueryResult _result;

            public WhenExecuteValidQuery()
            {
                _fixture = new Fixture()
                    .WithValidNoteQueryProcess();
            }

            public async Task InitializeAsync()
            {
                _query = new FetchOnePasswordNoteQuery
                {
                    UserId = "testUserId",
                    Environment = "DEV",
                    Vault = "Development",
                    Item = "Demo.AppSettings",
                    SessionToken = "FakeToken"
                };

                var sut = _fixture.CreateSubject();

                _result = await sut.Execute(_query);
            }

            [Fact]
            public void ShouldRunProcessToGetQuery()
            {
                var expectedSettings = new ProcessSettings
                {
                    Arguments = new ProcessArgumentBuilder()
                        .Append("item")
                        .Append("get")
                        .Append(_query.Item)
                        .Append("--vault")
                        .Append(_query.Vault)
                        .Append("--format")
                        .Append("json"),
                    EnvironmentVariables = new Dictionary<string, string>()
                        .AddValue($"OP_SESSION_{_query.UserId}", _query.SessionToken)
                };

                _fixture.ProcessRunner.VerifyRunCalledWith("op", expectedSettings);
            }

            [Fact]
            public void ShouldBeValidResult()
            {
                var expected = _fixture.CreateValidNoteQueryResult();
                _result.Should().BeEquivalentTo(expected);
            }

            public Task DisposeAsync() => Task.CompletedTask;
        }

        public class WhenAnyErrorProcessingQuery
        {
            [Fact]
            public async Task ShouldThrowWhenAnyErrorProcessingQuery()
            {
                var fixture = new Fixture().WithFailedProcessResult();

                var sut = fixture.CreateSubject();

                var query = new FetchOnePasswordNoteQuery();

                var act = async () => { await sut.Execute(query); };

                await act.Should().ThrowAsync<OnePasswordToolException>();
            }
        }
    }

    public class SignInOnePasswordCommandTests
    {
        public class WhenFirstTimeSigninCommand : IAsyncLifetime
        {
            readonly Fixture _fixture;
            SignInOnePasswordCommand _command;
            SignInOnePasswordResult _signInResult;
            readonly ProcessSettings _accountAddAndSigninSettings;

            public WhenFirstTimeSigninCommand()
            {
                _command = new SignInOnePasswordCommand
                {
                    IsFirstSignIn = true,
                    Organization = "appy",
                    Email = "test@yourorg.com",
                    SecretKey = "FakeKey"
                };

                var accountListSettings = new ProcessSettings
                {
                    Arguments = new ProcessArgumentBuilder()
                        .Append("account")
                        .Append("list")
                        .Append("--format")
                        .Append("json")
                };

                var accountListResult = new List<OnePasswordAccount> { new() { Shorthand = "appyop-appy", User_uuid = "testUserId" } };

                _accountAddAndSigninSettings = new ProcessSettings
                {
                    Arguments = new ProcessArgumentBuilder()
                        .Append("account")
                        .Append("add")
                        .Append("--address")
                        .Append($"https://{_command.Organization}.1password.com")
                        .Append("--email")
                        .Append(_command.Email!)
                        .Append("--secret-key")
                        .Append(_command.SecretKey!)
                        .Append("--shorthand")
                        .Append($"appyop-{_command.Organization}")
                        .Append($"--signin")
                        .Append($"--raw"),
                    EnvironmentVariables = new Dictionary<string, string>(),
                    RedirectStandardInput = false,
                    CreateNoWindow = false
                };

                _fixture = new Fixture()
                    .WithValidAccountListProcess(accountListSettings, accountListResult)
                    .WithValidSigninProcess(_accountAddAndSigninSettings);
            }

            public async Task InitializeAsync()
            {
                var sut = _fixture.CreateSubject();

                _signInResult = await sut.Execute(_command);
            }

            [Fact]
            public void ShouldAppendAddAccountAndSigninParametersAndRunProcessToExecuteCommand()
            {
                _fixture.ProcessRunner.VerifyRunCalledWith("op", _accountAddAndSigninSettings);
            }

            [Fact]
            public void ShouldBeValidResult()
            {
                var expected = SignInOnePasswordResult.Create(_fixture.UserId, _fixture.SessionToken);
                _signInResult.Should().BeEquivalentTo(expected);
            }

            public Task DisposeAsync() => Task.CompletedTask;
        }

        public class WhenLaterTimeSigninCommand : IAsyncLifetime
        {
            readonly Fixture _fixture;
            SignInOnePasswordCommand _command;
            SignInOnePasswordResult _result;

            public WhenLaterTimeSigninCommand()
            {
                _fixture = new Fixture()
                    .WithAnyValidSigninProcess();
            }

            public async Task InitializeAsync()
            {
                _command = new SignInOnePasswordCommand
                {
                    IsFirstSignIn = false,
                    Organization = "appy",
                    UserId = "testUserId"
                };

                var sut = _fixture.CreateSubject();

                _result = await sut.Execute(_command);
            }

            public Task DisposeAsync() => Task.CompletedTask;

            [Fact]
            public void ShouldAppendSigninWithAccountShorthandAndRunProcessToExecuteCommand()
            {
                var expectedSettings = new ProcessSettings
                {
                    Arguments = new ProcessArgumentBuilder()
                        .Append("signin")
                        .Append("--account")
                        .Append($"appyop-{_command.Organization}")
                        .Append("--raw"),
                    EnvironmentVariables = new Dictionary<string, string>(),
                    RedirectStandardInput = false,
                    CreateNoWindow = false
                };

                _fixture.ProcessRunner.VerifyRunCalledWith("op", expectedSettings);
            }

            [Fact]
            public void ShouldNotCallOnePasswordToGetAccountList()
            {
                var accountListSettings = new ProcessSettings
                {
                    Arguments = new ProcessArgumentBuilder()
                        .Append("account")
                        .Append("list")
                        .Append("--format")
                        .Append("json")
                };

                _fixture.ProcessRunner.VerifyRunNotCalledWith("op", accountListSettings);
            }

            [Fact]
            public void ShouldBeValidResult()
            {
                var expected = SignInOnePasswordResult.Create(_fixture.UserId, _fixture.SessionToken);
                _result.Should().BeEquivalentTo(expected);
            }
        }

        public class WhenAnyErrorProcessingCommand
        {
            [Fact]
            public async Task ShouldThrowException()
            {
                var fixture = new Fixture().WithFailedProcessResult();

                var sut = fixture.CreateSubject();

                var command = new SignInOnePasswordCommand
                {
                    IsFirstSignIn = true,
                    Organization = "appy",
                    Email = "test@yourorg.com",
                    SecretKey = "FakeKey"
                };

                var act = async () => { await sut.Execute(command); };

                await act.Should().ThrowAsync<OnePasswordToolException>();
            }
        }
    }
    public class Fixture
    {
        readonly IAppyJsonSerializer _serializer;
        public string UserId { get; }
        public string SessionToken { get; }
        public string FieldName { get; }
        public string FieldValue { get; }
        public Fixture()
        {
            _serializer = new NewtonsoftAppyJsonSerializer();

            Logger = new Mock<ILogger>();
            ProcessRunner = new ProcessRunnerMock();

            UserId = "testUserId";
            SessionToken = "FakeToken";
            FieldName = "Database";
            FieldValue = "Test";
        }

        public Mock<ILogger> Logger { get; }
        public ProcessRunnerMock ProcessRunner { get; }

        public IOnePasswordTool CreateSubject() =>
            new OnePasswordLocalTool(Logger.Object, _serializer, ProcessRunner.Object);

        public Fixture WithValidNoteQueryProcess()
        {
            var serialized = _serializer.Serialize(CreateValidNote());
            var result = ProcessResult.Create(serialized, "", 0);
            ProcessRunner.SetupRunAndReturns(result);
            return this;
        }

        public Fixture WithFailedProcessResult()
        {
            ProcessRunner.SetupRunAndReturns(ProcessResult.Create("", "", 1));
            return this;
        }

        public Fixture WithValidAccountListProcess(ProcessSettings settings, IReadOnlyCollection<OnePasswordAccount> accounts)
        {
            var serialized = _serializer.Serialize(accounts);
            var processResult = ProcessResult.Create(serialized, "", 0);
            ProcessRunner.SetupRunOpAndReturns(settings, processResult);
            return this;
        }

        public Fixture WithAnyValidSigninProcess()
        {
            var processResult = ProcessResult.Create($"{SessionToken}\n", "", 0);
            ProcessRunner.SetupRunAndReturns(processResult);
            return this;
        }

        public Fixture WithValidSigninProcess(ProcessSettings settings)
        {
            var processResult = ProcessResult.Create($"{SessionToken}\n", "", 0);
            ProcessRunner.SetupRunOpAndReturns(settings, processResult);
            return this;
        }

        static OnePasswordInternalSection CreateSection(string label) => new() { Label = label };

        public OnePasswordInternalNote CreateValidNote() =>
            new()
            {
                Fields = new List<OnePasswordInternalField>()
                    .AddItem(OnePasswordInternalField.New(FieldName, FieldValue, CreateSection("DEV")))
                    .AddItem(OnePasswordInternalField.New(FieldName, FieldValue, CreateSection("QA")))
            };

        public FetchOnePasswordNoteQueryResult CreateValidNoteQueryResult() =>
            new FetchOnePasswordNoteQueryResult()
                .WithTitle("DEV")
                .WithFields(new List<OnePasswordField>()
                    .AddItem(OnePasswordField.New(FieldName, FieldValue)));
    }
}