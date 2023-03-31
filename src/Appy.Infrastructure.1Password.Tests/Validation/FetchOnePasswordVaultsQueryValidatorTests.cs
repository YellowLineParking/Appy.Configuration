using Appy.Infrastructure.OnePassword.Queries;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Validation;

public class FetchOnePasswordVaultsQueryValidatorTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("testUserId", "")]
    public void ShouldBeInvalidWhenAnyPropertyIsEmpty(string userId, string sessionToken)
    {
        var sut = new FetchOnePasswordVaultsQueryValidator();

        var query = new FetchOnePasswordVaultsQuery
        {
            UserId = userId,
            SessionToken = sessionToken
        };
        var result = sut.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
    }

    [Theory]
    [InlineData("testUserId", "FakeToken")]
    public void ShouldBeValidWhenAllRequiredPropertiesHaveBeenSet(string userId, string sessionToken)
    {
        var sut = new FetchOnePasswordVaultsQueryValidator();

        var query = new FetchOnePasswordVaultsQuery
        {
            UserId = userId,
            SessionToken = sessionToken
        };
        var result = sut.Validate(query);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNull();
    }
}