using Appy.Tool.OnePassword.CLI;
using Appy.Tool.OnePassword.Composition;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword
{
    class Program
    {
        static int Main(string[] args) =>
            new ServiceCollection()
                .AddToolDependencies()
                .BuildServiceProvider()
                .GetService<IAppyOnePasswordToolCLI>()
                .ExecuteAsync(args).GetAwaiter().GetResult();
    }
}