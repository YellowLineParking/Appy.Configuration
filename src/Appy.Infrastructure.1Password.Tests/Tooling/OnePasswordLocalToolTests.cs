using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appy.Configuration.Common;
using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tests.Tooling.Fixtures;
using Appy.Infrastructure.OnePassword.Tooling;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling
{
    public class OnePasswordLocalToolTests
    {
        public class GetOnePasswordNoteQueryTests
        {
            public class WhenExecuteValidQuery : IAsyncLifetime
            {
                readonly Fixture _fixture;
                GetOnePasswordNoteQuery _query;
                GetOnePasswordNoteQueryResult _result;

                public WhenExecuteValidQuery()
                {
                    _fixture = new Fixture().WithValidNoteQueryProcess();
                }

                public async Task InitializeAsync()
                {
                    _query = new GetOnePasswordNoteQuery
                    {
                        Organization = "appy",
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
                            .Append("get")
                            .Append("item")
                            .Append(_query.Item)
                            .Append("--vault")
                            .Append(_query.Vault),
                        EnvironmentVariables = new Dictionary<string, string>()
                            .AddValue($"OP_SESSION_{_query.Organization}", _query.SessionToken)
                    };

                    _fixture.ProcessRunner.VerifyRunWith("op", expectedSettings);
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

                    var query = new GetOnePasswordNoteQuery();
                    Func<Task> act = async () => { await sut.Execute(query); };

                    await act.Should().ThrowAsync<OnePasswordToolException>();
                }
            }
        }

        public class SignInOnePasswordCommandTests
        {
            public class WhenExecuteValidCommand : IAsyncLifetime
            {
                readonly Fixture _fixture;
                SignInOnePasswordCommand _command;
                SignInOnePasswordResult _result;

                public WhenExecuteValidCommand()
                {
                    _fixture = new Fixture().WithValidSigninProcess();
                }

                public async Task InitializeAsync()
                {
                    _command = new SignInOnePasswordCommand
                    {
                        Organization = "appy",
                        IsFirstSignIn = true,
                        Email = "test@yourorg.com",
                        SecretKey = "FakeKey"
                    };

                    var sut = _fixture.CreateSubject();

                    _result = await sut.Execute(_command);
                }

                public Task DisposeAsync() => Task.CompletedTask;

                [Fact]
                public void ShouldRunProcessToExecuteCommand()
                {
                    var expectedSettings = new ProcessSettings
                    {
                        Arguments = new ProcessArgumentBuilder()
                            .Append("signin")
                            .Append($"https://{_command.Organization}.1password.com")
                            .Append(_command.Email!)
                            .Append(_command.SecretKey!)
                            .Append("--raw"),
                        EnvironmentVariables = new Dictionary<string, string>(),
                        RedirectStandardInput = false,
                        CreateNoWindow = false
                    };

                    _fixture.ProcessRunner.VerifyRunWith("op", expectedSettings);
                }

                [Fact]
                public void ShouldBeValidResult()
                {
                    var expected = SignInOnePasswordResult.Create(_fixture.SessionToken);
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
                        Organization = "appy",
                        IsFirstSignIn = true,
                        Email = "test@yourorg.com",
                        SecretKey = "FakeKey"
                    };

                    Func<Task> act = async () => { await sut.Execute(command); };

                    await act.Should().ThrowAsync<OnePasswordToolException>();
                }
            }
        }
        public class Fixture
        {
            public string SessionToken { get; set; }
            public string FieldName { get; set; }
            public string FieldValue { get; set; }
            public Fixture()
            {
                Logger = new Mock<ILogger>();
                Serializer = new JsonSerializerMock();
                ProcessRunner = new ProcessRunnerMock();

                SessionToken = "FakeToken";
                FieldName = "Database";
                FieldValue = "Test";
            }

            public Mock<ILogger> Logger { get; }
            public JsonSerializerMock Serializer { get; }
            public ProcessRunnerMock ProcessRunner { get; }

            public IOnePasswordTool CreateSubject()
            {
                return new OnePasswordLocalTool(Logger.Object, Serializer.Object, ProcessRunner.Object);
            }

            public Fixture WithValidNoteQueryProcess()
            {
                ProcessRunner.SetupAndReturns(ProcessResult.Create("", "", 0));
                Serializer.SetupAndReturns(CreateValidNote());
                return this;
            }

            public Fixture WithFailedProcessResult()
            {
                ProcessRunner.SetupAndReturns(ProcessResult.Create("", "", 1));
                return this;
            }

            public Fixture WithValidSigninProcess()
            {
                ProcessRunner.SetupAndReturns(ProcessResult.Create($"{SessionToken}\n", "", 0));
                return this;
            }

            OnePasswordSection CreateSection(string title) =>
                new OnePasswordSection()
                    .WithTitle(title)
                    .WithFields(new List<OnePasswordInternalField>()
                        .AddItem(OnePasswordInternalField.New(FieldName, FieldValue)));

            public OnePasswordNote CreateValidNote() =>
                new OnePasswordNote()
                    .WithDetails(new OnePasswordNoteDetails()
                        .WithSections(new List<OnePasswordSection>()
                            .AddItem(CreateSection("DEV"))
                            .AddItem(CreateSection("QA"))));

            public GetOnePasswordNoteQueryResult CreateValidNoteQueryResult() =>
                new GetOnePasswordNoteQueryResult()
                    .WithTitle("DEV")
                    .WithFields(new List<OnePasswordField>()
                        .AddItem(OnePasswordField.New(FieldName, FieldValue)));

        }
    }
}