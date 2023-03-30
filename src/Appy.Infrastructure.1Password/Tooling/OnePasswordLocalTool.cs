using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.Common;
using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;

namespace Appy.Infrastructure.OnePassword.Tooling;

///<inheritdoc cref="IOnePasswordTool"/>
public class OnePasswordLocalTool : IOnePasswordTool
{
    readonly ILogger _logger;
    readonly IAppyJsonSerializer _serializer;
    readonly IProcessRunner _processRunner;
    const string ToolPath = "op";

    public OnePasswordLocalTool(
        ILogger logger,
        IAppyJsonSerializer serializer,
        IProcessRunner processRunner)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    static Task<ProcessResult> GetItem(IProcessRunner processRunner, string organization, string vault, string item, string sessionToken)
    {
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("get")
                .Append("item")
                .Append(item)
                .Append("--vault")
                .Append(vault),
            EnvironmentVariables = new Dictionary<string, string>()
                .AddValue($"OP_SESSION_{organization}", sessionToken)
        };

        return processRunner.Run(ToolPath, processSettings);
    }

    static Task<ProcessResult> GetVaults(IProcessRunner processRunner, string organization, string sessionToken)
    {
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("list")
                .Append("vaults"),
            EnvironmentVariables = new Dictionary<string, string>()
                .AddValue($"OP_SESSION_{organization}", sessionToken)
        };

        return processRunner.Run(ToolPath, processSettings);
    }

    static Task<ProcessResult> Signin(IProcessRunner processRunner, ILogger logger, SignInOnePasswordCommand command)
    {
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
            Arguments = new ProcessArgumentBuilder()
                .Append("signin")
                .Append($"https://{command.Organization}.1password.com")
                .AppendIf(command.IsFirstSignIn, command.Email!)
                .AppendIf(command.IsFirstSignIn, command.SecretKey!)
                .Append("--raw"),
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

        processResult.ValidateAndThrow();

        var note = _serializer.DeserializeAndThrow<OnePasswordNote>(processResult.StandardOutput);

        var envSection = note?.GetSectionByEnvironment(query.Environment);

        return new GetOnePasswordNoteQueryResult
        {
            Title = envSection?.Title!,
            Fields = envSection?.Fields?.Select(f => new OnePasswordField
            {
                Name = f.T,
                Value = f.V
            }).ToList()
        };
    }

    ///<inheritdoc cref="IOnePasswordTool"/>
    public async Task<GetOnePasswordVaultsQueryResult> Execute(GetOnePasswordVaultsQuery query, CancellationToken cancellationToken = default)
    {
        var processResult = await GetVaults(
            processRunner: _processRunner,
            organization: query.Organization,
            sessionToken: query.SessionToken);

        processResult.ValidateAndThrow();

        var vaults = _serializer.DeserializeAndThrow<IList<OnePasswordVault>>(processResult.StandardOutput);

        return new GetOnePasswordVaultsQueryResult
        {
            Vaults = vaults
        };
    }

    ///<inheritdoc cref="IOnePasswordTool"/>
    public async Task<SignInOnePasswordResult> Execute(SignInOnePasswordCommand command, CancellationToken cancellationToken = default)
    {
        var processResult = await Signin(_processRunner, _logger, command);

        processResult.ValidateAndThrow();

        var sessionToken = processResult.StandardOutput.TrimEnd('\n');

        return SignInOnePasswordResult.Create(sessionToken);
    }
}