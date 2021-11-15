using Appy.Tool.OnePassword.Cli;
using Appy.Tool.OnePassword.Composition;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword;

internal static class Program
{
    static int Main(string[] args) =>
        new ServiceCollection()
        .AddToolDependencies()
        .BuildServiceProvider()
        .GetService<IAppyOnePasswordToolCli>()
        .ExecuteAsync(args).GetAwaiter().GetResult();
}