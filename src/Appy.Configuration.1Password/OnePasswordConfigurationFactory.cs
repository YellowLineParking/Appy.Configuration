using Appy.Configuration.IO;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Tooling;

namespace Appy.Configuration.OnePassword
{
    internal static class OnePasswordConfigurationFactory
    {
        public static IAppyJsonSerializer CreateDefaultSerializer() =>
            new NewtonsoftAppyJsonSerializer();

        public static IProcessRunner CreateDefaultProcessRunner() =>
            new DefaultProcessRunner();

        public static IOnePasswordTool CreateDefaultOnePasswordTool(
            IAppyJsonSerializer jsonSerializer,
            IProcessRunner processRunner,
            IOnePasswordUserEnvironmentAccessor userEnvironmentAccessor) =>
            new OnePasswordTool(
                jsonSerializer,
                processRunner);

        public static IOnePasswordUserEnvironmentAccessor CreateDefaultUserEnvironmentAccessor() =>
            new OnePasswordUserEnvironmentAccessor();
    }
}