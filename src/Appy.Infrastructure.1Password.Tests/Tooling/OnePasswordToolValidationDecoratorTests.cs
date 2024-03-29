using System;
using System.Threading.Tasks;
using Appy.Configuration.Validation;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tests.Fixtures;
using Appy.Infrastructure.OnePassword.Tooling;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling;

public class OnePasswordToolValidationDecoratorTests
{
    public class FetchOnePasswordNoteQueryTests
    {
        public class WhenExecuteInvalidQuery
        {
            [Fact]
            public async Task ShouldValidateAndThrow()
            {
                var fixture = new Fixture().WithInvalidValidationResult<FetchOnePasswordNoteQuery>();

                var query = new FetchOnePasswordNoteQuery();

                var sut = fixture.CreateSubject();

                Func<Task> act = async () => await sut.Execute(query);

                await act.Should().ThrowAsync<ValidationException>();
            }
        }

        public class WhenExecuteValidQuery
        {
            [Fact]
            public async Task ShouldValidateAndCallInnerTool()
            {
                var fixture = new Fixture().WithValidValidationResult<FetchOnePasswordNoteQuery>();

                var query = new FetchOnePasswordNoteQuery
                {
                    UserId = fixture.UserId,
                    Item = fixture.Item,
                    Vault = fixture.Vault,
                    Environment = fixture.Environment,
                    SessionToken = fixture.SessionToken
                };

                var sut = fixture.CreateSubject();

                var result = await sut.Execute(query);

                fixture.InnerTool.VerifyCalledWith(query);
            }
        }
    }

    public class FetchOnePasswordVaultsQueryTests
    {
        public class WhenExecuteInvalidQuery
        {
            [Fact]
            public async Task ShouldValidateAndThrow()
            {
                var fixture = new Fixture().WithInvalidValidationResult<FetchOnePasswordVaultsQuery>();

                var query = new FetchOnePasswordVaultsQuery();

                var sut = fixture.CreateSubject();

                Func<Task> act = async () => await sut.Execute(query);

                await act.Should().ThrowAsync<ValidationException>();
            }
        }

        public class WhenExecuteValidQuery
        {
            [Fact]
            public async Task ShouldValidateAndCallInnerTool()
            {
                var fixture = new Fixture().WithValidValidationResult<FetchOnePasswordVaultsQuery>();

                var query = new FetchOnePasswordVaultsQuery
                {
                    UserId = fixture.UserId,
                    SessionToken = fixture.SessionToken
                };

                var sut = fixture.CreateSubject();

                var result = await sut.Execute(query);

                fixture.InnerTool.VerifyCalledWith(query);
            }
        }
    }

    public class SignInOnePasswordCommandTests
    {
        public class WhenExecuteInvalidCommand
        {
            [Fact]
            public async Task ShouldValidateAndThrow()
            {
                var fixture = new Fixture().WithInvalidValidationResult<SignInOnePasswordCommand>();

                var command = new SignInOnePasswordCommand();

                var sut = fixture.CreateSubject();

                Func<Task> act = async () => await sut.Execute(command);

                await act.Should().ThrowAsync<ValidationException>();
            }
        }

        public class WhenExecuteValidCommand
        {
            [Fact]
            public async Task ShouldValidateAndCallInnerTool()
            {
                var fixture = new Fixture().WithValidValidationResult<SignInOnePasswordCommand>();

                var command = new SignInOnePasswordCommand
                {
                    Organization = fixture.Organization
                };

                var sut = fixture.CreateSubject();

                await sut.Execute(command);

                fixture.InnerTool.VerifyCalledWith(command);
            }
        }
    }

    public class Fixture
    {
        public string Organization { get; }
        public string UserId { get; }
        public string Environment { get; }
        public string Vault { get; }
        public string Item { get; }
        public string SessionToken { get; }

        public Fixture()
        {
            InnerTool = new OnePasswordToolMock();
            Validator = new ValidatorMock();
            Organization = "appy";
            UserId = "testUserId";
            Environment = "DEV";
            Vault = "Development";
            Item = "Demo.AppSettings";
            SessionToken = "FakeToken";
        }

        public OnePasswordToolMock InnerTool { get; }

        public ValidatorMock Validator { get; }

        public IOnePasswordTool CreateSubject() =>
            InnerTool.Object.WithValidation(Validator.Object);

        public Fixture WithValidValidationResult<TModel>()
        {
            Validator.SetupAndReturns<TModel>(new ValidationResult());
            return this;
        }

        public Fixture WithInvalidValidationResult<TModel>()
        {
            Validator.SetupAndReturns<TModel>(
                new ValidationResult().WithError(string.Empty, string.Empty));
            return this;
        }
    }
}