using System;
using System.Collections.Generic;
using System.Linq;
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

            var tasks = new List<Task>();

            var command = Command.Run(toolPath, settings.Arguments, options =>
            {
                if (settings.EnvironmentVariables?.Count > 0)
                {
                    options.EnvironmentVariables(settings.EnvironmentVariables);
                }

                options.StartInfo(pinfo =>
                {
                    pinfo.RedirectStandardInput = settings.RedirectStandardInput;
                    pinfo.CreateNoWindow = settings.CreateNoWindow;
                    pinfo.UseShellExecute = settings.UseShellExecute;
                });
            });

            if (settings.StandardErrorReader != null)
            {
                tasks.Add(settings.StandardErrorReader.Invoke(command.StandardError));
            }

            tasks.Add(command.Task);

            await Task.WhenAll(tasks.ToArray());

            var commandResult = await (Task<CommandResult>)tasks.Last();

            return ProcessResult.Create(
                standardOutput: commandResult.StandardOutput,
                standardError: commandResult.StandardError,
                exitCode: commandResult.ExitCode
            );
        }
    }
}