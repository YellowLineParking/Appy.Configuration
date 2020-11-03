using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.TestTools;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling.Fixtures
{
    public class OnePasswordSessionStorageMock: Mock<IOnePasswordSessionStorage>
    {
        public OnePasswordSessionStorageMock SetupAndReturns(AppyOnePasswordSession session)
        {
            this.Setup(x => x.GetCurrent())
                .ReturnsAsync(session);
            return this;
        }

        public void VerifyUpdateWith(AppyOnePasswordSession expected)
        {
            Verify(x => x.Update(
                It.Is<AppyOnePasswordSession>(session =>
                    session.IsEquivalentTo(expected))));
        }
    }
}