using Appy.Infrastructure.OnePassword.Queries;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Validation;

public class FetchOnePasswordNoteQueryValidatorTests
{
    [Theory]
    [InlineData("", "", "", "", "")]
    [InlineData("Appy.Fake", "", "", "", "")]
    [InlineData("Appy.Fake", "testUserId", "", "", "")]
    [InlineData("Appy.Fake", "testUserId", "Development", "", "")]
    [InlineData("Appy.Fake", "testUserId", "Development", "DEV", "")]
    public void ShouldBeInvalidWhenAnyPropertyIsEmpty(
        string item, string userId, string vault, string environment, string sessionToken)
    {
        var sut = new FetchOnePasswordNoteQueryValidator();

        var query = new FetchOnePasswordNoteQuery
        {
            Item = item,
            UserId = userId,
            Vault = vault,
            Environment = environment,
            SessionToken = sessionToken
        };
        var result = sut.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
    }

    [Theory]
    [InlineData("Appy.Fake", "testUserId", "Development", "DEV", "FakeToken")]
    public void ShouldBeValidWhenAllRequiredPropertiesHaveBeenSet(
        string item, string userId, string vault, string environment, string sessionToken)
    {
        var sut = new FetchOnePasswordNoteQueryValidator();

        var query = new FetchOnePasswordNoteQuery
        {
            Item = item,
            UserId = userId,
            Vault = vault,
            Environment = environment,
            SessionToken = sessionToken
        };
        var result = sut.Validate(query);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNull();
    }
}