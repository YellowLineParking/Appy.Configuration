using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Tooling.Internal;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Fixtures;

public class JsonSerializerMock : Mock<IAppyJsonSerializer>
{
    public JsonSerializerMock SetupAndReturns(OnePasswordInternalNote note)
    {
        Setup(x => x.Deserialize<OnePasswordInternalNote>(
                It.IsAny<string>()))
            .Returns(note);
        return this;
    }
}