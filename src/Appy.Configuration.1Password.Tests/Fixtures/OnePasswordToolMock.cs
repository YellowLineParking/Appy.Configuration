using System.Threading;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.TestTools;
using Moq;

namespace Appy.Configuration.OnePassword.Tests.Fixtures
{
    public class OnePasswordToolMock : Mock<IOnePasswordTool>
    {
        public OnePasswordToolMock SetupAndReturns(GetOnePasswordNoteQueryResult result)
        {
            Setup(x => x.Execute(
                It.IsAny<GetOnePasswordNoteQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(result);
            return this;
        }

        public void VerifyCalledWith(GetOnePasswordNoteQuery expectedQuery)
        {
            Verify(x => x.Execute(
                It.Is<GetOnePasswordNoteQuery>(q => q.IsEquivalentTo(expectedQuery)),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}