using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.IO;
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
        readonly IAppyJsonSerializer _jsonSerializer;
        readonly IProcessRunner _processRunner;
        const string ToolPath = "op";

        public OnePasswordTool(
            IAppyJsonSerializer jsonSerializer,
            IProcessRunner processRunner)
        {
            _jsonSerializer = jsonSerializer;
            _processRunner = processRunner;
        }

        static async Task<ProcessResult> GetItem(IProcessRunner processRunner, string organization, string vault, string item, string sessionToken)
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

            var processResult = await processRunner.Run(ToolPath, processSettings);

            return processResult;
        }

        ///<inheritdoc cref="IOnePasswordTool"/>
        public async Task<GetOnePasswordNoteQueryResult> Execute(GetOnePasswordNoteQuery query, CancellationToken cancellationToken = default)
        {
            var processResult = await GetItem(
                processRunner: _processRunner,
                organization: query.Organisation,
                vault: query.Vault,
                item: query.Item,
                sessionToken: query.SessionToken);

            if (!processResult.Success)
            {
                throw new OnePasswordToolException($"1Password internal CLI failed with exit code {processResult.ExitCode}: {processResult.StandardError}");
            }

            OnePasswordSection? environmentSection;

            try
            {
                var note = _jsonSerializer.Deserialize<OnePasswordNote>(processResult.StandardOutput);
                environmentSection = note?.GetSectionByEnvironment(query.Environment);
            }
            catch (Exception ex)
            {
                throw new OnePasswordToolException("1Password Tool failed deserialization", innerException: ex);
            }

            if (environmentSection == null || environmentSection.Fields?.Count == 0)
            {
                throw new OnePasswordToolException($"1Password ${query.Environment} Environment Section does not contain a valid configuration");
            }

            var queryResult = new GetOnePasswordNoteQueryResult
            {
                EnvironmentSection = environmentSection
            };

            return queryResult;
        }

        static ValidationResult Validate(SignInOnePasswordCommand command)
        {
            var result = new ValidationResult();
            if (string.IsNullOrWhiteSpace(command.Organization))
                return result.WithError(nameof(command.Organization).ToLower(), "1Password organization must be specified");

            if (!command.IsFirstSignIn)
                return result;

            if (string.IsNullOrWhiteSpace(command.Email))
                return result.WithError(nameof(command.Email).ToLower(), "1Password email must be specified");

            if (string.IsNullOrWhiteSpace(command.SecretKey))
                return result.WithError(nameof(command.Email).ToLower(), "1Password secret key must be specified");

            return result;
        }

        static async Task<ProcessResult> Signin(IProcessRunner processRunner, SignInOnePasswordCommand command)
        {
            var arguments = new ProcessArgumentBuilder();
            arguments.Append("signin");
            arguments.Append($"https://{command.Organization}.1password.com");

            if (command.IsFirstSignIn)
            {
                arguments.Append(command.Email);
                arguments.Append(command.SecretKey);
            }

            arguments.Append("--raw");

            var processSettings = new ProcessSettings
            {
                Arguments = arguments,
                EnvironmentVariables = new Dictionary<string, string>()
            };

            var processResult = await processRunner.Run(ToolPath, processSettings);

            return processResult;
        }

        public async Task<SigninOnePasswordResult> Execute(SignInOnePasswordCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = Validate(command);
            if (!validationResult.IsValid)
            {
                return SigninOnePasswordResult.Create(string.Empty);
            }

            var processResult = await Signin(
                processRunner: _processRunner,
                command: command);

            if (!processResult.Success)
            {
                throw new OnePasswordToolException($"1Password internal CLI failed with exit code {processResult.ExitCode}: {processResult.StandardError}");
            }

            var commandResult = SigninOnePasswordResult.Create(processResult.StandardOutput);

            return commandResult;
        }
    }
}