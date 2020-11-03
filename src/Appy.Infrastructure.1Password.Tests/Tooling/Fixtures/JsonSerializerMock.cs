using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Model;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Tooling.Fixtures
{
    public class JsonSerializerMock : Mock<IAppyJsonSerializer>
    {
        public JsonSerializerMock SetupAndReturns(OnePasswordNote note)
        {
            Setup(x => x.Deserialize<OnePasswordNote>(
                    It.IsAny<string>()))
                .Returns(note);
            return this;
        }
    }
}