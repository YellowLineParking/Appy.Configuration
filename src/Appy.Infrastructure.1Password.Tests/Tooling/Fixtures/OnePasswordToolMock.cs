using System;
using System.Threading;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.TestTools;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling.Fixtures
{
    public class OnePasswordToolMock : Mock<IOnePasswordTool>
    {
        public OnePasswordToolMock SetupAndReturns(SignInOnePasswordResult result)
        {
            Setup(x => x.Execute(
                    It.IsAny<SignInOnePasswordCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }

        public OnePasswordToolMock SetupAndReturns(GetOnePasswordNoteQueryResult result)
        {
            Setup(x => x.Execute(
                    It.IsAny<GetOnePasswordNoteQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }

        public OnePasswordToolMock SetupAndThrows(Exception exception)
        {
            Setup(x => x.Execute(
                    It.IsAny<GetOnePasswordNoteQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return this;
        }

        public OnePasswordToolMock VerifyCalledWith(GetOnePasswordNoteQuery expected)
        {
            Verify(x => x.Execute(
                It.Is<GetOnePasswordNoteQuery>(q => q.IsEquivalentTo(expected)),
                It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public OnePasswordToolMock VerifyCalledWith(GetOnePasswordVaultsQuery expected)
        {
            Verify(x => x.Execute(
                It.Is<GetOnePasswordVaultsQuery>(q => q.IsEquivalentTo(expected)),
                It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public OnePasswordToolMock VerifyCalledWith(SignInOnePasswordCommand expected)
        {
            Verify(x => x.Execute(
                It.Is<SignInOnePasswordCommand>(q => q.IsEquivalentTo(expected)),
                It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }
    }
}