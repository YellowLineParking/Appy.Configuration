using Appy.Configuration.IO;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Tooling;

namespace Appy.Configuration.OnePassword
{
    internal class OnePasswordConfigurationFactory
    {
        public static IConfigurationJsonSerializer CreateDefaultSerializer() =>
            new NewtonsoftConfigurationJsonSerializer();

        public static IProcessRunner CreateDefaultProcessRunner() =>
            new DefaultProcessRunner();

        public static IOnePasswordTool CreateDefaultOnePasswordTool(IConfigurationJsonSerializer jsonSerializer, IProcessRunner processRunner) =>
            new OnePasswordTool(jsonSerializer, processRunner);
    }
}