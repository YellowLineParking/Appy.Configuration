using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.TestTools;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling
{
    public class OnePasswordToolTests
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
                    _fixture = new Fixture();
                }

                public async Task InitializeAsync()
                {
                    _fixture.SetupWithValidNoteQueryProcess();
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

                public Task DisposeAsync() => Task.CompletedTask;


                [Fact]
                public void ShouldRunProcessToGetQuery()
                {
                    var arguments = new ProcessArgumentBuilder();
                    arguments.Append("get");
                    arguments.Append("item");
                    arguments.Append(_query.Item);
                    arguments.Append("--vault");
                    arguments.Append(_query.Vault);

                    var expectedSettings = new ProcessSettings
                    {
                        Arguments = arguments,
                        EnvironmentVariables = new Dictionary<string, string>
                        {
                            {$"OP_SESSION_{_query.Organization}", _query.SessionToken}
                        }
                    };

                    _fixture.ProcessRunner.Verify(x => x.Run(
                        It.Is<string>(tool => tool == "op"),
                        It.Is<ProcessSettings>(ps => ps.IsEquivalentTo(expectedSettings))));
                }

                [Fact]
                public void ShouldBeValidResult()
                {
                    var expected = _fixture.CreateValidNoteQueryResult();

                    _result.Should().BeEquivalentTo(expected);
                }
            }

            [Fact]
            public async Task ShouldThrowWhenAnyErrorProcessingQuery()
            {
                var fixture = new Fixture();
                fixture.SetupWithFailedProcessResult();
                var sut = fixture.CreateSubject();

                var query = new GetOnePasswordNoteQuery();
                Func<Task> act = async () => { await sut.Execute(query); };

                await act.Should().ThrowAsync<OnePasswordToolException>();
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
                    _fixture = new Fixture();
                }

                public async Task InitializeAsync()
                {
                    _fixture.SetupWithValidSigninProcess();
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

                static bool IsEquivalentTo(ProcessSettings source, ProcessSettings expected)
                {
                    return source.Arguments.IsEquivalentTo(expected.Arguments) &&
                           source.EnvironmentVariables.IsEquivalentTo(expected.EnvironmentVariables) &&
                           source.RedirectStandardInput == expected.RedirectStandardInput &&
                           source.CreateNoWindow == expected.CreateNoWindow;
                }

                [Fact]
                public void ShouldRunProcessToExecuteCommand()
                {
                    var arguments = new ProcessArgumentBuilder();
                    arguments.Append("signin");
                    arguments.Append($"https://{_command.Organization}.1password.com");
                    arguments.Append(_command.Email!);
                    arguments.Append(_command.SecretKey!);
                    arguments.Append("--raw");

                    var expectedSettings = new ProcessSettings
                    {
                        Arguments = arguments,
                        EnvironmentVariables = new Dictionary<string, string>(),
                        RedirectStandardInput = false,
                        CreateNoWindow = false
                    };

                    _fixture.ProcessRunner.Verify(x => x.Run(
                        It.Is<string>(tool => tool == "op"),
                        It.Is<ProcessSettings>(ps => IsEquivalentTo(ps, expectedSettings))));
                }

                [Fact]
                public void ShouldBeValidResult()
                {
                    var expected = SignInOnePasswordResult.Create(_fixture.SessionToken);

                    _result.Should().BeEquivalentTo(expected);
                }
            }

            [Fact]
            public async Task ShouldThrowWhenAnyErrorProcessingCommand()
            {
                var fixture = new Fixture();
                fixture.SetupWithFailedProcessResult();
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
        public class Fixture
        {
            public string SessionToken { get; set; }
            public string FieldName { get; set; }
            public string FieldValue { get; set; }
            public Fixture()
            {
                Logger = new Mock<ILogger>();
                Serializer = new Mock<IAppyJsonSerializer>();
                ProcessRunner = new Mock<IProcessRunner>();

                SessionToken = "FakeToken";
                FieldName = "Database";
                FieldValue = "Test";
            }

            public Mock<ILogger> Logger { get; }
            public Mock<IAppyJsonSerializer> Serializer { get; }
            public Mock<IProcessRunner> ProcessRunner { get; }

            public IOnePasswordTool CreateSubject()
            {
                return new OnePasswordTool(Logger.Object, Serializer.Object, ProcessRunner.Object);
            }

            public void SetupWithValidNoteQueryProcess()
            {
                var processResult = ProcessResult.Create("", "", 0);
                var note = CreateValidNote();

                ProcessRunner.Setup(x => x.Run(
                        It.IsAny<string>(),
                        It.IsAny<ProcessSettings>()))
                    .ReturnsAsync(processResult);

                Serializer.Setup(x => x.Deserialize<OnePasswordNote>(
                        It.IsAny<string>()))
                    .Returns(note);
            }

            public void SetupWithFailedProcessResult()
            {
                var processResult = ProcessResult.Create("", "", 1);

                ProcessRunner.Setup(x => x.Run(
                    It.IsAny<string>(),
                    It.IsAny<ProcessSettings>()))
                    .ReturnsAsync(processResult);
            }

            public void SetupWithValidSigninProcess()
            {
                var processResult = ProcessResult.Create($"{SessionToken}\n", "", 0);

                ProcessRunner.Setup(x => x.Run(
                        It.IsAny<string>(),
                        It.IsAny<ProcessSettings>()))
                    .ReturnsAsync(processResult);
            }

            OnePasswordSection CreateSection(string title)
            {
                return new OnePasswordSection
                {
                    Title = title,
                    Fields = new List<OnePasswordField> { new OnePasswordField { T = FieldName, V = FieldValue } }
                };
            }

            public OnePasswordNote CreateValidNote()
            {
                return new OnePasswordNote
                {
                    Details = new OnePasswordNoteDetails
                    {
                        Sections = new List<OnePasswordSection>
                        {
                            CreateSection("DEV"), CreateSection("QA")
                        }
                    }
                };
            }
            public GetOnePasswordNoteQueryResult CreateValidNoteQueryResult()
            {
                return new GetOnePasswordNoteQueryResult
                {
                    EnvironmentSection = CreateSection("DEV")
                };
            }
        }
    }
}