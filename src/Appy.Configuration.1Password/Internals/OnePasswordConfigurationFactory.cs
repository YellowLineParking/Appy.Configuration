using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Tooling;

namespace Appy.Configuration.OnePassword.Internals
{
    internal static class OnePasswordConfigurationFactory
    {
        internal static ILogger EmptyLogger() => new EmptyLogger();

        internal static IAppyJsonSerializer CreateDefaultSerializer() =>
            new NewtonsoftAppyJsonSerializer();

        internal static IProcessRunner CreateDefaultProcessRunner() =>
            new DefaultProcessRunner();

        internal static IOnePasswordTool CreateDefaultOnePasswordTool(
            IAppyJsonSerializer jsonSerializer,
            IProcessRunner processRunner,
            ILogger logger) =>
            new OnePasswordTool(
                logger,
                jsonSerializer,
                processRunner);

        internal static IOnePasswordUserEnvironmentAccessor CreateDefaultUserEnvironmentAccessor() =>
            new OnePasswordUserEnvironmentAccessor();
    }
}