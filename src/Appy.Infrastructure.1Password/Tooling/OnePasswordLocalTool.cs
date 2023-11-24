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
using Appy.Infrastructure.OnePassword.Tooling.Internal;

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

    static Task<ProcessResult> GetItem(IProcessRunner processRunner, string userId, string vault, string item, string sessionToken)
    {
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("item")
                .Append("get")
                .Append(item)
                .Append("--vault")
                .Append(vault)
                .WithJsonFormat(),
            EnvironmentVariables = new Dictionary<string, string>()
                .AddValue($"OP_SESSION_{userId}", sessionToken)
        };

        return processRunner.Run(ToolPath, processSettings);
    }

    static Task<ProcessResult> GetVaults(IProcessRunner processRunner, string userId, string sessionToken)
    {
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("vault")
                .Append("list")
                .WithJsonFormat(),
            EnvironmentVariables = new Dictionary<string, string>()
                .AddValue($"OP_SESSION_{userId}", sessionToken)
        };

        return processRunner.Run(ToolPath, processSettings);
    }

    static Task<ProcessResult> Signin(IProcessRunner processRunner, ILogger logger, SignInOnePasswordCommand command)
    {
        var processSettings = new ProcessSettings
        {
            Arguments = CreateArgumentsFrom(command),
            EnvironmentVariables = new Dictionary<string, string>(),
            RedirectStandardInput = false,
            CreateNoWindow = false,
            StandardErrorReader = sr => LogLines(logger, textReader: sr, stopChar: ':')
        };

        return processRunner.Run(ToolPath, processSettings);

        static async Task LogLines(ILogger logger, TextReader textReader, char stopChar)
        {
            var stringBuilder = new StringBuilder();

            while (true)
            {
                var hasMore = await textReader.WriteLineAsyncTo(stringBuilder, stopChar);
                if (hasMore == null)
                    return;

                logger.LogInformation(stringBuilder.ToString());

                stringBuilder.Clear();

                if (!hasMore.Value)
                    return;
            }
        }
    }

    static ProcessArgumentBuilder CreateArgumentsFrom(SignInOnePasswordCommand command)
    {
        var processArgumentBuilder = new ProcessArgumentBuilder();

        return command.IsFirstSignIn
            ? processArgumentBuilder.WithAddAccountAndSigninParameters(command)
            : processArgumentBuilder.WithSigninWithAccountParameters(command.Organization);
    }

    async Task<IReadOnlyCollection<OnePasswordAccount>> FetchAllAccounts()
    {
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("account")
                .Append("list")
                .WithJsonFormat()
        };

        var processResult = await _processRunner.Run(ToolPath, processSettings);

        processResult.ValidateAndThrow();

        return _serializer.DeserializeAndThrow<IReadOnlyCollection<OnePasswordAccount>>(processResult.StandardOutput);
    }

    async Task<string> FetchCurrentUserId(SignInOnePasswordCommand command)
    {
        if (!command.IsFirstSignIn)
            return command.UserId!;

        var accounts = await FetchAllAccounts();

        var accountShorthand = OnePasswordAccountHelper.ShorthandForOrg(command.Organization);

        var current = accounts.FirstOrDefault(x => x.Shorthand.Equals(accountShorthand, StringComparison.OrdinalIgnoreCase));

        if (current == null)
            throw new InvalidOperationException($"Could not find a 1Password account with the shorthand '{accountShorthand}'.");

        return current.User_uuid;
    }

    ///<inheritdoc cref="IOnePasswordTool"/>
    public async Task<FetchOnePasswordNoteQueryResult> Execute(FetchOnePasswordNoteQuery query, CancellationToken cancellationToken = default)
    {
        var processResult = await GetItem(
            processRunner: _processRunner,
            userId: query.UserId,
            vault: query.Vault,
            item: query.Item,
            sessionToken: query.SessionToken);

        processResult.ValidateAndThrow();

        var note = _serializer.DeserializeAndThrow<OnePasswordInternalNote>(processResult.StandardOutput);

        var envFields = note?.GetFieldsForEnvironment(query.Environment);

        return new FetchOnePasswordNoteQueryResult
        {
            Title = query.Environment,
            Fields = envFields?.Select(field => new OnePasswordField
            {
                Name = field.Label,
                Value = field.Value
            }).ToList()
        };
    }

    ///<inheritdoc cref="IOnePasswordTool"/>
    public async Task<FetchOnePasswordVaultsQueryResult> Execute(FetchOnePasswordVaultsQuery query, CancellationToken cancellationToken = default)
    {
        var processResult = await GetVaults(
            processRunner: _processRunner,
            userId: query.UserId,
            sessionToken: query.SessionToken);

        processResult.ValidateAndThrow();

        var vaults = _serializer.DeserializeAndThrow<IReadOnlyCollection<OnePasswordVault>>(processResult.StandardOutput);

        return new FetchOnePasswordVaultsQueryResult
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

        var userId = await FetchCurrentUserId(command);

        return SignInOnePasswordResult.Create(userId, sessionToken);
    }
}