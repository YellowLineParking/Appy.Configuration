using Appy.Infrastructure.OnePassword.Queries;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Validation;

public class GetOnePasswordVaultsQueryValidatorTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("appy", "")]
    public void ShouldBeInvalidWhenAnyPropertyIsEmpty(string organization, string sessionToken)
    {
        var sut = new GetOnePasswordVaultsQueryValidator();

        var query = new GetOnePasswordVaultsQuery
        {
            Organization = organization,
            SessionToken = sessionToken
        };
        var result = sut.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
    }

    [Theory]
    [InlineData("appy", "FakeToken")]
    public void ShouldBeValidWhenAllRequiredPropertiesHaveBeenSet(string organization, string sessionToken)
    {
        var sut = new GetOnePasswordVaultsQueryValidator();

        var query = new GetOnePasswordVaultsQuery
        {
            Organization = organization,
            SessionToken = sessionToken
        };
        var result = sut.Validate(query);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNull();
    }
}