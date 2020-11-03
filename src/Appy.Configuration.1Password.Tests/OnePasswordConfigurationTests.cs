using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.OnePassword;
using Appy.Configuration.OnePassword.Internals;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.TestTools;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Appy.Configuration.OnePassword.Tests
{
    public class OnePasswordConfigurationTests
    {
        public class ConfigurationSource
        {
            [Theory]
            [InlineData("", "", "", "", "")]
            [InlineData("Appy.Fake", "", "", "", "")]
            [InlineData("Appy.Fake", "appy", "", "", "")]
            [InlineData("Appy.Fake", "appy", "Development", "", "")]
            [InlineData("Appy.Fake", "appy", "Development", "DEV", "")]
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
                configurationSource.AppSettingsName.Should().Be($"{fixture.AppName}.AppSettings");
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
                var fixture = new Fixture();
                fixture.SetupWithNoteQueryResult(fixture.ValidNoteQueryResult);

                var sut = fixture.CreateSubject();

                sut.Load();

                var expectedQuery = fixture.ValidNoteQuery;
                fixture.Tool.Verify(t => t.Execute(
                    It.Is<GetOnePasswordNoteQuery>(q => q.IsEquivalentTo(expectedQuery)),
                        It.IsAny<CancellationToken>()));
                sut.Get(fixture.FieldName).Should().Be(fixture.FieldValue);
            }

            [Theory]
            [MemberData(nameof(InvalidNoteQueryResults))]
            public void ShouldThrowWhenInvalidNoteQueryResult(GetOnePasswordNoteQueryResult result)
            {
                var fixture = new Fixture();

                fixture.SetupWithNoteQueryResult(result);

                var sut = fixture.CreateSubject();

                Action act = () => sut.Load();

                act.Should().Throw<OnePasswordConfigurationException>();
            }

            public static IEnumerable<object[]> InvalidNoteQueryResults()
            {
                yield return new object[] { new GetOnePasswordNoteQueryResult() };
                yield return new object[] { new GetOnePasswordNoteQueryResult
                {
                    EnvironmentSection = new OnePasswordSection()
                }};
                yield return new object[] { new GetOnePasswordNoteQueryResult
                {
                    EnvironmentSection = new OnePasswordSection
                    {
                        Fields = new List<OnePasswordField> { new OnePasswordField() }
                    }
                }};
                yield return new object[] { new GetOnePasswordNoteQueryResult
                {
                    EnvironmentSection = new OnePasswordSection
                    {
                        Fields = new List<OnePasswordField>
                        {
                            new OnePasswordField { T = "Database" }
                        }
                    }
                }};
            }
        }

        private class Fixture
        {
            public string AppName { get; set; }
            public string Organization { get; set; }
            public string Vault { get; set; }
            public string Environment { get; set; }
            public string SessionToken { get; set; }

            public string FieldName { get; set; }

            public string FieldValue { get; set; }

            public GetOnePasswordNoteQuery ValidNoteQuery { get; }
            public GetOnePasswordNoteQueryResult ValidNoteQueryResult { get; }
            public Mock<IOnePasswordTool> Tool { get; }

            public Fixture()
            {
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
                    EnvironmentSection = new OnePasswordSection
                    {
                        Fields = new List<OnePasswordField>
                        {
                            new OnePasswordField { T = FieldName, V = FieldValue }
                        }
                    }
                };

                Tool = new Mock<IOnePasswordTool>();
            }

            public OnePasswordConfigurationProvider CreateSubject()
            {
                return new OnePasswordConfigurationProvider(
                    new OnePasswordConfigurationSource(Tool.Object, AppName, Organization, Vault, Environment, SessionToken));
            }

            public void SetupWithNoteQueryResult(GetOnePasswordNoteQueryResult result)
            {
                Tool.Setup(x => x.Execute(
                        It.IsAny<GetOnePasswordNoteQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(result);
            }
        }
    }
}