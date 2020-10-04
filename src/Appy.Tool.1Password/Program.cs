using Appy.Tool.OnePassword.CLI;
using Appy.Tool.OnePassword.Composition;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword
{
    class Program
    {
        static int Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddToolDependencies();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var toolCLI = serviceProvider.GetService<IAppyOnePasswordToolCLI>();
            var exitCode = toolCLI.ExecuteAsync(args).GetAwaiter().GetResult();
            return exitCode;
        }
    }
}