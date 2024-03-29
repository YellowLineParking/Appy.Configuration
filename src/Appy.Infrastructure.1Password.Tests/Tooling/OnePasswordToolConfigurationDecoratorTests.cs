using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tests.Fixtures;
using Appy.Infrastructure.OnePassword.Tooling;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling;

public class OnePasswordToolConfigurationDecoratorTests
{
    public class FetchOnePasswordNoteQueryTests
    {
        public class WhenExecuteQueryWithOnlyRequiredProperties
        {
            [Fact]
            public async Task ShouldDecorateQueryWithMissingPropertiesFromCurrentSession()
            {
                var fixture = new Fixture()
                    .WithCurrentSession();

                var query = new FetchOnePasswordNoteQuery
                {
                    Item = "Demo.AppSettings",
                };

                var sut = fixture.CreateSubject();

                var expectedInnerQuery = new FetchOnePasswordNoteQuery
                {
                    UserId = fixture.UserId,
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
        public string Organization { get; }
        public string UserId { get; }
        public string Environment { get; }
        public string Vault { get; }
        public string Item { get; }
        public string SessionToken { get; }

        public Fixture()
        {
            InnerTool = new OnePasswordToolMock();
            SessionStorage = new OnePasswordSessionStorageMock();
            Organization = "appy";
            UserId = "testUserId";
            Environment = "DEV";
            Vault = "Development";
            Item = "Demo.AppSettings";
            SessionToken = "FakeToken";
        }

        public OnePasswordToolMock InnerTool { get; }
        public OnePasswordSessionStorageMock SessionStorage { get; }

        public IOnePasswordTool CreateSubject() =>
            InnerTool.Object.WithConfiguration(SessionStorage.Object);

        public Fixture WithCurrentSession()
        {
            SessionStorage.SetupGetAndReturns(AppyOnePasswordSession.New(
                organization: Organization,
                userId: UserId,
                vault: Vault,
                environment: Environment,
                sessionToken: SessionToken));

            return this;
        }
    }
}