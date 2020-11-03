using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tests.Tooling.Fixtures;
using Appy.Infrastructure.OnePassword.Tooling;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling
{
    public class OnePasswordToolConfigurationDecoratorTests
    {
        public class GetOnePasswordNoteQueryTests
        {
            public class WhenExecuteQueryWithOnlyRequiredProperties
            {
                [Fact]
                public async Task ShouldDecorateQueryWithMissingPropertiesFromCurrentSession()
                {
                    var fixture = new Fixture()
                        .WithCurrentSession();

                    var query = new GetOnePasswordNoteQuery
                    {
                        Item = "Demo.AppSettings",
                    };

                    var sut = fixture.CreateSubject();

                    var expectedInnerQuery = new GetOnePasswordNoteQuery
                    {
                        Organization = fixture.Organization,
                        Item = query.Item,
                        Vault = fixture.Vault,
                        Environment = fixture.Environment,
                        SessionToken = fixture.SessionToken
                    };

                    var result = await sut.Execute(query);

                    fixture.InnerTool.VerifyCalledWith(expectedInnerQuery);
                }
            }
        }

        public class Fixture
        {
            public string Organization { get; set; }
            public string Environment { get; set; }
            public string Vault { get; set; }
            public string Item { get; set; }
            public string SessionToken { get; set; }

            public Fixture()
            {
                InnerTool = new OnePasswordToolMock();
                SessionStorage = new OnePasswordSessionStorageMock();
                Organization = "appy";
                Environment = "DEV";
                Vault = "Development";
                Item = "Demo.AppSettings";
                SessionToken = "FakeToken";
            }

            public OnePasswordToolMock InnerTool { get; }
            public OnePasswordSessionStorageMock SessionStorage { get; }

            public IOnePasswordTool CreateSubject()
            {
                return InnerTool.Object.WithConfiguration(SessionStorage.Object);
            }
            public Fixture WithCurrentSession()
            {
                SessionStorage.SetupAndReturns(AppyOnePasswordSession.New(
                    organization: Organization,
                    vault: Vault,
                    environment: Environment,
                    sessionToken: SessionToken));

                return this;
            }
        }
    }
}