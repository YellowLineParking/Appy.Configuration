using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Validation;

namespace Appy.Infrastructure.OnePassword.Tooling
{
    ///<inheritdoc cref="IOnePasswordTool"/>
    public class OnePasswordTool : IOnePasswordTool
    {
        readonly ILogger _logger;
        readonly IAppyJsonSerializer _jsonSerializer;
        readonly IProcessRunner _processRunner;
        const string ToolPath = "op";

        public OnePasswordTool(
            ILogger logger,
            IAppyJsonSerializer jsonSerializer,
            IProcessRunner processRunner)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        }

        static Task<ProcessResult> GetItem(IProcessRunner processRunner, string organization, string vault, string item, string sessionToken)
        {
            var arguments = new ProcessArgumentBuilder();
            arguments.Append("get");
            arguments.Append("item");
            arguments.Append(item);
            arguments.Append("--vault");
            arguments.Append(vault);

            var processSettings = new ProcessSettings
            {
                Arguments = arguments,
                EnvironmentVariables = new Dictionary<string, string>()
            };

            var sessionEnvName = $"OP_SESSION_{organization}";

            processSettings.EnvironmentVariables.Add(sessionEnvName, sessionToken);

            return processRunner.Run(ToolPath, processSettings);
        }

        static Task<ProcessResult> GetVaults(IProcessRunner processRunner, string organization, string sessionToken)
        {
            var arguments = new ProcessArgumentBuilder();
            arguments.Append("list");
            arguments.Append("vaults");

            var processSettings = new ProcessSettings
            {
                Arguments = arguments,
                EnvironmentVariables = new Dictionary<string, string>()
            };

            var sessionEnvName = $"OP_SESSION_{organization}";

            processSettings.EnvironmentVariables.Add(sessionEnvName, sessionToken);

            return processRunner.Run(ToolPath, processSettings);
        }

        static Task<ProcessResult> Signin(IProcessRunner processRunner, ILogger logger, SignInOnePasswordCommand command)
        {
            var arguments = new ProcessArgumentBuilder();
            arguments.Append("signin");
            arguments.Append($"https://{command.Organization}.1password.com");

            if (command.IsFirstSignIn)
            {
                arguments.Append(command.Email!);
                arguments.Append(command.SecretKey!);
            }

            arguments.Append("--raw");

            static async Task LogLines(ILogger logger, TextReader sr)
            {
                while (true)
                {
                    var line = await sr.ReadLineAsync(new StringBuilder());
                    if (line == null) return;
                    logger.LogInformation(line.Result);
                    if (!line.HasMore) return;
                }
            }

            var processSettings = new ProcessSettings
            {
                Arguments = arguments,
                EnvironmentVariables = new Dictionary<string, string>(),
                RedirectStandardInput = false,
                CreateNoWindow = false,
                StandardErrorReader = sr => LogLines(logger, sr)
            };

            return processRunner.Run(ToolPath, processSettings);
        }

        ///<inheritdoc cref="IOnePasswordTool"/>
        public async Task<GetOnePasswordNoteQueryResult> Execute(GetOnePasswordNoteQuery query, CancellationToken cancellationToken = default)
        {
            var processResult = await GetItem(
                processRunner: _processRunner,
                organization: query.Organization,
                vault: query.Vault,
                item: query.Item,
                sessionToken: query.SessionToken);

            if (!processResult.Success)
            {
                throw new OnePasswordToolException($"1Password internal CLI failed with exit code {processResult.ExitCode}: {processResult.StandardError}");
            }

            try
            {
                var note = _jsonSerializer.Deserialize<OnePasswordNote>(processResult.StandardOutput);

                var environmentSection = note?.GetSectionByEnvironment(query.Environment);

                return new GetOnePasswordNoteQueryResult
                {
                    EnvironmentSection = environmentSection
                };
            }
            catch (Exception ex)
            {
                throw new OnePasswordToolException("1Password Tool failed deserialization", ex);
            }
        }

        ///<inheritdoc cref="IOnePasswordTool"/>
        public async Task<GetOnePasswordVaultsQueryResult> Execute(GetOnePasswordVaultsQuery query, CancellationToken cancellationToken = default)
        {
            var processResult = await GetVaults(
                processRunner: _processRunner,
                organization: query.Organization,
                sessionToken: query.SessionToken);

            if (!processResult.Success)
            {
                throw new OnePasswordToolException($"1Password internal CLI failed with exit code {processResult.ExitCode}: {processResult.StandardError}");
            }

            try
            {
                var vaults = _jsonSerializer.Deserialize<IList<OnePasswordVault>>(processResult.StandardOutput);

                return new GetOnePasswordVaultsQueryResult
                {
                    Vaults = vaults
                };
            }
            catch (Exception ex)
            {
                throw new OnePasswordToolException("1Password Tool failed deserialization", ex);
            }
        }

        ///<inheritdoc cref="IOnePasswordTool"/>
        public async Task<SignInOnePasswordResult> Execute(SignInOnePasswordCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();
            if (!validationResult.IsValid)
            {
                return SignInOnePasswordResult.Create(string.Empty);
            }

            var processResult = await Signin(_processRunner, _logger, command);

            if (!processResult.Success)
            {
                throw new OnePasswordToolException($"1Password internal CLI failed with exit code {processResult.ExitCode}: {processResult.StandardError}");
            }

            var sessionToken = processResult.StandardOutput.TrimEnd('\n');

            return SignInOnePasswordResult.Create(sessionToken);
        }
    }
}