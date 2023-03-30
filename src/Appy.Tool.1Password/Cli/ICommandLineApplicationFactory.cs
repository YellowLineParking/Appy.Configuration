using McMaster.Extensions.CommandLineUtils;

namespace Appy.Tool.OnePassword.Cli;

public interface ICommandLineApplicationFactory
{
    CommandLineApplication Create(string name, string fullName);
}