using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.TestTools;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Fixtures;

public class OnePasswordSessionStorageMock: Mock<IOnePasswordSessionStorage>
{
    public void SetupGetAndReturns(AppyOnePasswordSession session)
    {
        this.Setup(x => x.GetCurrent())
            .ReturnsAsync(session);
    }

    public void VerifyUpdateWith(AppyOnePasswordSession expected)
    {
        Verify(x => x.Update(
            It.Is<AppyOnePasswordSession>(session =>
                session.IsEquivalentTo(expected))));
    }

    public void VerifyGetNotCalled()
    {
        Verify(x => x.GetCurrent(),
            Times.Exactly(0));
    }

    public void VerifyUpdateNotCalled()
    {
        Verify(x => x.Update(
                It.IsAny<AppyOnePasswordSession>()),
            Times.Exactly(0));
    }
}