using McMaster.Extensions.CommandLineUtils;

namespace Appy.Tool.OnePassword.Cli
{
    public class CommandLineApplicationFactory : ICommandLineApplicationFactory
    {
        public CommandLineApplication Create(string name, string fullName)
        {
            return new CommandLineApplication { Name = name, FullName = fullName};
        }
    }
}