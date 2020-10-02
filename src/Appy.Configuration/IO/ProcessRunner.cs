using System;
using System.Threading.Tasks;
using Medallion.Shell;

namespace Appy.Configuration.IO
{
    public class DefaultProcessRunner : IProcessRunner
    {
        public async Task<ProcessResult> Run(string toolPath, ProcessSettings settings)
        {
            if (string.IsNullOrEmpty(toolPath)) throw new ArgumentNullException(nameof(toolPath));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (settings.Arguments == null) throw new ArgumentNullException(nameof(settings.Arguments));

            var command = Command.Run(toolPath, settings.Arguments, options =>
            {
                if (settings.EnvironmentVariables?.Count > 0)
                {
                    options.EnvironmentVariables(settings.EnvironmentVariables);
                }
            });

            var commandResult = await command.Task;

            return ProcessResult.Create(
                standardOutput: commandResult.StandardOutput,
                standardError: commandResult.StandardError,
                exitCode: commandResult.ExitCode
            );
        }
    }
}