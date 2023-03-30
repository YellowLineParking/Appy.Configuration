using Appy.Infrastructure.OnePassword.Queries;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Validation;

public class GetOnePasswordNoteQueryValidatorTests
{
    [Theory]
    [InlineData("", "", "", "", "")]
    [InlineData("Appy.Fake", "", "", "", "")]
    [InlineData("Appy.Fake", "appy", "", "", "")]
    [InlineData("Appy.Fake", "appy", "Development", "", "")]
    [InlineData("Appy.Fake", "appy", "Development", "DEV", "")]
    public void ShouldBeInvalidWhenAnyPropertyIsEmpty(
        string item, string organization, string vault, string environment, string sessionToken)
    {
        var sut = new GetOnePasswordNoteQueryValidator();

        var query = new GetOnePasswordNoteQuery
        {
            Item = item,
            Organization = organization,
            Vault = vault,
            Environment = environment,
            SessionToken = sessionToken
        };
        var result = sut.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
    }

    [Theory]
    [InlineData("Appy.Fake", "appy", "Development", "DEV", "FakeToken")]
    public void ShouldBeValidWhenAllRequiredPropertiesHaveBeenSet(
        string item, string organization, string vault, string environment, string sessionToken)
    {
        var sut = new GetOnePasswordNoteQueryValidator();

        var query = new GetOnePasswordNoteQuery
        {
            Item = item,
            Organization = organization,
            Vault = vault,
            Environment = environment,
            SessionToken = sessionToken
        };
        var result = sut.Validate(query);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNull();
    }
}