using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Infrastructure.OnePassword.Validation;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests
{
    public class ValidationExtensionTests
    {
        [Fact]
        public void ShouldBeInvalidWhenOrganizationIsEmpty()
        {
            var command = new SignInOnePasswordCommand();

            var result = command.Validate();

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldBeValidWhenOrganizationAndNotFirstSignIn()
        {
            var command = new SignInOnePasswordCommand
            {
                Organization = "appy"
            };

            var result = command.Validate();

            result.IsValid.Should().BeTrue();
            result.Errors.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldBeInvalidWhenOrganizationAndFirstSignIn()
        {
            var command = new SignInOnePasswordCommand
            {
                Organization = "appy",
                IsFirstSignIn = true
            };

            var result = command.Validate();

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldBeInvalidWhenOrganizationAndEmailIsEmpty()
        {
            var command = new SignInOnePasswordCommand
            {
                Organization = "appy",
                IsFirstSignIn = true,
                Email = "test@appy.com"
            };

            var result = command.Validate();

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldBeValidWhenRequiredParamsAreSetted()
        {
            var command = new SignInOnePasswordCommand
            {
                Organization = "appy",
                IsFirstSignIn = true,
                Email = "test@appy.com",
                SecretKey = "secretKey"
            };

            var result = command.Validate();

            result.IsValid.Should().BeTrue();
            result.Errors.Count.Should().Be(0);
        }
    }
}