using Appy.Infrastructure.OnePassword.Commands;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Validation
{
    public class SignInOnePasswordCommandValidatorTests
    {
        [Fact]
        public void ShouldBeInvalidWhenOrganizationIsEmpty()
        {
            var sut = new SignInOnePasswordCommandValidator();

            var command = new SignInOnePasswordCommand();
            var result = sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldBeValidWhenOrganizationAndNotFirstSignIn()
        {
            var sut = new SignInOnePasswordCommandValidator();

            var command = new SignInOnePasswordCommand {Organization = "appy"};
            var result = sut.Validate(command);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeNull();
        }

        [Fact]
        public void ShouldBeInvalidWhenOrganizationAndFirstSignIn()
        {
            var sut = new SignInOnePasswordCommandValidator();

            var command = new SignInOnePasswordCommand {Organization = "appy", IsFirstSignIn = true};
            var result = sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldBeInvalidWhenOrganizationAndEmailIsEmpty()
        {
            var sut = new SignInOnePasswordCommandValidator();

            var command = new SignInOnePasswordCommand
            {
                Organization = "appy", IsFirstSignIn = true, Email = "test@appy.com"
            };
            var result = sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldBeValidWhenAllRequiredPropertiesHaveBeenSet()
        {
            var sut = new SignInOnePasswordCommandValidator();

            var command = new SignInOnePasswordCommand
            {
                Organization = "appy",
                IsFirstSignIn = true,
                Email = "test@appy.com",
                SecretKey = "secretKey"
            };
            var result = sut.Validate(command);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeNull();
        }
    }
}