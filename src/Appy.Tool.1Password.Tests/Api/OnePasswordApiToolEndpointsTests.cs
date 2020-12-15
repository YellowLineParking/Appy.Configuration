using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appy.Configuration;
using Appy.Configuration.Common;
using Appy.Configuration.Validation;
using Appy.Infrastructure.OnePassword.ApiClient;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tests.Fixtures;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Tool.OnePassword.Tests.Api.Fixtures;
using FluentAssertions;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Appy.Tool.OnePassword.Tests.Api
{
    public static class OnePasswordApiToolEndpointsTests
    {
        public class WhenRemoteToolAndValidNoteQuery : IClassFixture<OnePasswordApiTestFixture>, IDisposable
        {
            readonly OnePasswordApiTestFixture _apiTestFixture;

            public WhenRemoteToolAndValidNoteQuery(OnePasswordApiTestFixture apiTestFixture, ITestOutputHelper output)
            {
                _apiTestFixture = apiTestFixture.WithOutput(output);
            }

            [Fact]
            public async Task ShouldBeSuccessfulResponse()
            {
                var expected = new GetOnePasswordNoteQueryResult()
                    .WithTitle("DEV")
                    .WithFields(new List<OnePasswordField>()
                        .AddItem(OnePasswordField.New("TestName", "TestValue")));

                var fixture = new Fixture(_apiTestFixture).WithNoteQueryResult(expected);
                var query = fixture.CreateValidOnePasswordNoteQuery();

                var result = await fixture.RemoteTool.Execute(query);

                fixture.LocalTool.VerifyCalledWith(query);
                result.Should().BeEquivalentTo(expected);
            }

            public void Dispose() => _apiTestFixture.Dispose();
        }

        public class WhenRemoteToolAndInvalidNoteQuery : IClassFixture<OnePasswordApiTestFixture>, IDisposable
        {
            readonly OnePasswordApiTestFixture _apiTestFixture;

            public WhenRemoteToolAndInvalidNoteQuery(OnePasswordApiTestFixture apiTestFixture, ITestOutputHelper output)
            {
                _apiTestFixture = apiTestFixture.WithOutput(output);
            }

            [Fact]
            public async Task ShouldBeUnsuccessfulResponseWithValidationInfo()
            {
                var fixture = new Fixture(_apiTestFixture);
                var query = fixture.CreateInvalidOnePasswordNoteQuery();

                Func<Task> act = async () => await fixture.RemoteTool.Execute(query);

                var assertion = await act.Should().ThrowAsync<OnePasswordToolException>();
                assertion.WithInnerException<FlurlHttpException>();

                var response = assertion.And.GetContentAs<Response>();
                response.Success.Should().BeFalse();
                response.Message.Should().Be("The request is invalid");
                response.Errors.Count.Should().Be(1);
                response.Errors.First().Property.Should().Be("item");
                response.Errors.First().Message.Should().Be("1Password AppName must be specified.");
            }

            public void Dispose() => _apiTestFixture.Dispose();
        }

        public class WhenRemoteToolAndLocalToolException : IClassFixture<OnePasswordApiTestFixture>, IDisposable
        {
            readonly OnePasswordApiTestFixture _apiTestFixture;

            public WhenRemoteToolAndLocalToolException(OnePasswordApiTestFixture apiTestFixture, ITestOutputHelper output)
            {
                _apiTestFixture = apiTestFixture.WithOutput(output);
            }

            [Fact]
            public async Task ShouldBeUnsuccessfulResponseWithLocalExceptionInfo()
            {
                var localException = new OnePasswordToolException("Tool test error");

                var fixture = new Fixture(_apiTestFixture).WithLocalToolException(localException);
                var query = fixture.CreateValidOnePasswordNoteQuery();

                Func<Task> act = async () => await fixture.RemoteTool.Execute(query);

                var assertion = await act.Should().ThrowAsync<OnePasswordToolException>();
                assertion.WithInnerException<FlurlHttpException>();

                var response = assertion.And.GetContentAs<Response>();
                response.Success.Should().BeFalse();
                response.Errors.Should().BeNull();
                response.Message.Should().Be(localException.Message);
            }

            public void Dispose() => _apiTestFixture.Dispose();
        }

        public class WhenRemoteToolAndLocalInternalError : IClassFixture<OnePasswordApiTestFixture>, IDisposable
        {
            readonly OnePasswordApiTestFixture _apiTestFixture;

            public WhenRemoteToolAndLocalInternalError(OnePasswordApiTestFixture apiTestFixture, ITestOutputHelper output)
            {
                _apiTestFixture = apiTestFixture.WithOutput(output);
            }

            [Fact]
            public async Task ShouldBeUnsuccessfulResponseWithInternalErrorMessage()
            {
                var fixture = new Fixture(_apiTestFixture).WithLocalInternalError();
                var query = fixture.CreateValidOnePasswordNoteQuery();

                Func<Task> act = async () => await fixture.RemoteTool.Execute(query);

                var assertion = await act.Should().ThrowAsync<OnePasswordToolException>();
                assertion.WithInnerException<FlurlHttpException>();

                var response = assertion.And.GetContentAs<Response>();
                response.Success.Should().BeFalse();
                response.Message.Should().Be("Internal server error");
                response.Errors.Should().BeNull();
            }

            public void Dispose() => _apiTestFixture.Dispose();
        }

        public class Fixture : IDisposable
        {
            readonly OnePasswordApiTestFixture _apiTestFixture;
            public string Organization { get; }
            public string Environment { get; }
            public string Vault { get; }
            public string Item { get; }
            public string SessionToken { get; }

            public Fixture(OnePasswordApiTestFixture apiTestFixture)
            {
                _apiTestFixture = apiTestFixture;
                _apiTestFixture.ServicesConfiguration = services => services
                    .ReplaceSingleton<IOnePasswordTool>(sp =>
                        LocalTool.Object.WithValidation(sp.GetService<IValidator>()));

                LocalTool = new OnePasswordToolMock();
                RemoteTool = new OnePasswordRemoteTool(
                    new OnePasswordApiTestClientFactory(_apiTestFixture.CreateClient()));

                Organization = "appy";
                Environment = "DEV";
                Vault = "Development";
                Item = "Demo.AppSettings";
                SessionToken = "FakeToken";
            }

            public OnePasswordToolMock LocalTool { get; }

            public IOnePasswordTool RemoteTool { get; }

            public Fixture WithNoteQueryResult(GetOnePasswordNoteQueryResult result)
            {
                LocalTool.SetupAndReturns(result);
                return this;
            }

            public Fixture WithLocalToolException(OnePasswordToolException exception)
            {
                LocalTool.SetupAndThrows(exception);
                return this;
            }

            public Fixture WithLocalInternalError()
            {
                LocalTool.SetupAndThrows(new Exception("Error"));
                return this;
            }

            public GetOnePasswordNoteQuery CreateInvalidOnePasswordNoteQuery()
            {
                return new GetOnePasswordNoteQuery();
            }

            public GetOnePasswordNoteQuery CreateValidOnePasswordNoteQuery()
            {
                return new GetOnePasswordNoteQuery
                {
                    Organization = Organization,
                    Environment = Environment,
                    Vault = Vault,
                    Item = Item,
                    SessionToken = SessionToken
                };
            }

            public void Dispose()
            {
                _apiTestFixture?.Dispose();
            }
        }
    }
}