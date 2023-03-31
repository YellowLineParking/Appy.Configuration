using System.Threading;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.TestTools;
using Moq;

namespace Appy.Configuration.OnePassword.Tests.Fixtures;

public class OnePasswordToolMock : Mock<IOnePasswordTool>
{
    public OnePasswordToolMock SetupAndReturns(FetchOnePasswordNoteQueryResult result)
    {
        Setup(x => x.Execute(
            It.IsAny<FetchOnePasswordNoteQuery>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(result);
        return this;
    }

    public void VerifyCalledWith(FetchOnePasswordNoteQuery expectedQuery)
    {
        Verify(x => x.Execute(
            It.Is<FetchOnePasswordNoteQuery>(q => q.IsEquivalentTo(expectedQuery)),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}