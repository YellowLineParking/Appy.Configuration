using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appy.Configuration.IO;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;

namespace Appy.Infrastructure.OnePassword.Tooling
{
    ///<inheritdoc cref="IOnePasswordTool"/>
    public class OnePasswordTool : IOnePasswordTool
    {
        readonly IConfigurationJsonSerializer _jsonSerializer;
        readonly IProcessRunner _processRunner;
        const string ToolPath = "op";

        public OnePasswordTool(IConfigurationJsonSerializer jsonSerializer, IProcessRunner processRunner)
        {
            _jsonSerializer = jsonSerializer;
            _processRunner = processRunner;
        }

        static async Task<ProcessResult> ExecuteGet(IProcessRunner processRunner, string organization, string vault, string item, string sessionToken)
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
        public async Task<OnePasswordGetNoteQueryResult> Execute(OnePasswordGetNoteQuery query)
        {
            var processResult = await ExecuteGet(
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

                environmentSection = note?.Details?.Sections?
                    .SingleOrDefault(section => string.Equals(section.Title, query.Environment, StringComparison.InvariantCulture));
            }
            catch (Exception ex)
            {
                throw new OnePasswordToolException("1Password Tool failed deserialization", innerException: ex);
            }

            if (environmentSection == null || environmentSection.Fields?.Count == 0)
            {
                throw new OnePasswordToolException($"1Password ${query.Environment} Environment Section does not contain a valid configuration");
            }

            var queryResult = new OnePasswordGetNoteQueryResult
            {
                EnvironmentSection = environmentSection
            };

            return queryResult;
        }
    }
}