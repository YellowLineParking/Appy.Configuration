using System;
using System.Collections.Generic;
using Appy.Configuration.OnePassword.Tests.Fixtures;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.TestTools;
using FluentAssertions;
using Xunit;

namespace Appy.Configuration.OnePassword.Tests;

public class OnePasswordConfigurationTests
{
    public class ConfigurationSource
    {
        [Theory]
        [InlineData(null, "", "", "", "")]
        [InlineData("", "", "", "", "")]
        public void ShouldThrowArgumentExceptionWithInvalidParams(string appName, string userId, string vault,
            string environment, string sessionToken)
        {
            var fixture = new Fixture();

            Action act = () => new OnePasswordConfigurationSource(
                fixture.Tool.Object,
                appName,
                userId,
                vault,
                environment,
                sessionToken);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ShouldNotThrowWithValidParams()
        {
            var fixture = new Fixture();

            var configurationSource = new OnePasswordConfigurationSource(
                fixture.Tool.Object,
                fixture.AppName,
                fixture.UserId,
                fixture.Vault,
                fixture.Environment,
                fixture.SessionToken);

            configurationSource.AppName.Should().Be(fixture.AppName);
            configurationSource.UserId.Should().Be(fixture.UserId);
            configurationSource.Vault.Should().Be(fixture.Vault);
            configurationSource.Environment.Should().Be(fixture.Environment);
            configurationSource.SessionToken.Should().Be(fixture.SessionToken);
        }
    }

    public class ConfigurationProvider
    {
        [Fact]
        public void ShouldCallToolAndGetValidQueryResult()
        {
            var fixture = new Fixture().WithValidNoteQueryResult();
            var sut = fixture.CreateSubject();

            sut.Load();

            fixture.Tool.VerifyCalledWith(fixture.ValidNoteQuery);
            sut.Get(fixture.FieldName).Should().Be(fixture.FieldValue);
        }

        [Theory]
        [MemberData(nameof(InvalidNoteQueryResults))]
        public void ShouldThrowWhenInvalidNoteQueryResult(FetchOnePasswordNoteQueryResult result)
        {
            var fixture = new Fixture().WithNoteQueryResult(result);
            var sut = fixture.CreateSubject();

            var act = () => sut.Load();

            act.Should().Throw<OnePasswordConfigurationException>();
        }

        public static IEnumerable<object[]> InvalidNoteQueryResults()
        {
            yield return new object[] { new FetchOnePasswordNoteQueryResult() };
            yield return new object[] { new FetchOnePasswordNoteQueryResult
            {
                Fields = new List<OnePasswordField> { new() }
            }};
            yield return new object[] { new FetchOnePasswordNoteQueryResult
            {
                Fields = new List<OnePasswordField>
                {
                    new() { Name = "Database" }
                }
            }};
        }
    }

    private class Fixture
    {
        public OnePasswordToolMock Tool { get; }
        public string AppName { get; set; }
        public string Organization { get; set; }
        public string UserId { get; set; }
        public string Vault { get; set; }
        public string Environment { get; set; }
        public string SessionToken { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public FetchOnePasswordNoteQuery ValidNoteQuery { get; }
        public FetchOnePasswordNoteQueryResult ValidNoteQueryResult { get; }

        public Fixture()
        {
            Tool = new OnePasswordToolMock();
            AppName = "Appy.Fake";
            Organization = "appy";
            UserId = "testUserId";
            Vault = "Development";
            Environment = "DEV";
            SessionToken = "FakeToken";
            FieldName = "Database";
            FieldValue = "Test";

            ValidNoteQuery = new FetchOnePasswordNoteQuery
            {
                Item = $"{AppName}.AppSettings",
                UserId = UserId,
                Vault = Vault,
                Environment = Environment,
                SessionToken = SessionToken
            };

            ValidNoteQueryResult = new FetchOnePasswordNoteQueryResult
            {
                Title = "DEV",
                Fields = new List<OnePasswordField>
                {
                    new() { Name = FieldName, Value = FieldValue }
                }
            };
        }

        public OnePasswordConfigurationProvider CreateSubject() => new(
            new OnePasswordConfigurationSource(
                tool: Tool.Object,
                appName: AppName,
                userId: UserId,
                vault: Vault,
                environment: Environment,
                sessionToken: SessionToken));

        public Fixture WithNoteQueryResult(FetchOnePasswordNoteQueryResult result)
        {
            Tool.SetupAndReturns(result);
            return this;
        }

        public Fixture WithValidNoteQueryResult() => WithNoteQueryResult(ValidNoteQueryResult);
    }
}