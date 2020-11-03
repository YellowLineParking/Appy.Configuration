using System;
using System.Collections.Generic;
using Appy.Configuration.OnePassword.Tests.Fixtures;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.TestTools;
using FluentAssertions;
using Xunit;

namespace Appy.Configuration.OnePassword.Tests
{
    public class OnePasswordConfigurationTests
    {
        public class ConfigurationSource
        {
            [Theory]
            [InlineData(null, "", "", "", "")]
            [InlineData("", "", "", "", "")]
            public void ShouldThrowArgumentExceptionWithInvalidParams(string appName, string organization, string vault,
                string environment, string sessionToken)
            {
                var fixture = new Fixture();

                Action act = () => new OnePasswordConfigurationSource(
                    fixture.Tool.Object,
                    appName,
                    organization,
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
                    fixture.Organization,
                    fixture.Vault,
                    fixture.Environment,
                    fixture.SessionToken);

                configurationSource.AppName.Should().Be(fixture.AppName);
                configurationSource.Organization.Should().Be(fixture.Organization);
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
;               var fixture = new Fixture().WithValidNoteQueryResult();
                var sut = fixture.CreateSubject();

                sut.Load();

                fixture.Tool.VerifyCalledWith(fixture.ValidNoteQuery);
                sut.Get(fixture.FieldName).Should().Be(fixture.FieldValue);
            }

            [Theory]
            [MemberData(nameof(InvalidNoteQueryResults))]
            public void ShouldThrowWhenInvalidNoteQueryResult(GetOnePasswordNoteQueryResult result)
            {
                var fixture = new Fixture().WithNoteQueryResult(result);
                var sut = fixture.CreateSubject();

                Action act = () => sut.Load();

                act.Should().Throw<OnePasswordConfigurationException>();
            }

            public static IEnumerable<object[]> InvalidNoteQueryResults()
            {
                yield return new object[] { new GetOnePasswordNoteQueryResult() };
                yield return new object[] { new GetOnePasswordNoteQueryResult
                {
                    Fields = new List<OnePasswordField> { new OnePasswordField() }
                }};
                yield return new object[] { new GetOnePasswordNoteQueryResult
                {
                    Fields = new List<OnePasswordField>
                    {
                        new OnePasswordField { Name = "Database" }
                    }
                }};
            }
        }

        private class Fixture
        {
            public OnePasswordToolMock Tool { get; }
            public string AppName { get; set; }
            public string Organization { get; set; }
            public string Vault { get; set; }
            public string Environment { get; set; }
            public string SessionToken { get; set; }
            public string FieldName { get; set; }
            public string FieldValue { get; set; }
            public GetOnePasswordNoteQuery ValidNoteQuery { get; }
            public GetOnePasswordNoteQueryResult ValidNoteQueryResult { get; }

            public Fixture()
            {
                Tool = new OnePasswordToolMock();
                AppName = "Appy.Fake";
                Organization = "appy";
                Vault = "Development";
                Environment = "DEV";
                SessionToken = "FakeToken";
                FieldName = "Database";
                FieldValue = "Test";

                ValidNoteQuery = new GetOnePasswordNoteQuery
                {
                    Item = $"{AppName}.AppSettings",
                    Organization = Organization,
                    Vault = Vault,
                    Environment = Environment,
                    SessionToken = SessionToken
                };

                ValidNoteQueryResult = new GetOnePasswordNoteQueryResult
                {
                    Title = "DEV",
                    Fields = new List<OnePasswordField>
                    {
                        new OnePasswordField { Name = FieldName, Value = FieldValue }
                    }
                };
            }

            public OnePasswordConfigurationProvider CreateSubject()
            {
                return new OnePasswordConfigurationProvider(
                    new OnePasswordConfigurationSource(Tool.Object, AppName, Organization, Vault, Environment, SessionToken));
            }

            public Fixture WithNoteQueryResult(GetOnePasswordNoteQueryResult result)
            {
                Tool.SetupAndReturns(result);
                return this;
            }

            public Fixture WithValidNoteQueryResult() => WithNoteQueryResult(ValidNoteQueryResult);
        }
    }
}