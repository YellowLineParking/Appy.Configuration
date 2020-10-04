using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Tool.OnePassword.Logging;
using McMaster.Extensions.CommandLineUtils;

namespace Appy.Tool.OnePassword.CLI
{
    internal class AppyOnePasswordToolCLI : IAppyOnePasswordToolCLI
    {
        private readonly ILogger _logger;
        private readonly IOnePasswordUserEnvironmentAccessor _environmentAccessor;
        private readonly IOnePasswordTool _onePasswordTool;
        static string GetAssemblyVersion() => typeof(AppyOnePasswordToolCLI).Assembly.GetName().Version.ToString();
        public AppyOnePasswordToolCLI(
            ILogger logger,
            IOnePasswordUserEnvironmentAccessor environmentAccessor,
            IOnePasswordTool onePasswordTool
        )
        {
            _logger = logger;
            _environmentAccessor = environmentAccessor;
            _onePasswordTool = onePasswordTool;
        }

        SignInOnePasswordCommand BuildSignInCommand(CommandLineApplication app, CommandOption option)
        {
            if (!option.HasValue())
            {
                throw new CommandParsingException(app, "Signin option must be specified.");
            }

            var values = (string.IsNullOrWhiteSpace(option.Value())
                ? Enumerable.Empty<string>()
                : option.Value().Split(' ').Select(v => v.Trim('\"', ' '))).ToList();

            if (values.Count > 0 && values.Count != 3)
            {
                throw new CommandParsingException(app, "Signin values must be specified: <organization> <email_address> <secret_key>.");
            }

            if (values.Count > 0)
            {
                return new SignInOnePasswordCommand
                {
                    Organization = values.ElementAt(0),
                    Email = values.ElementAt(1),
                    SecretKey = values.ElementAt(2),
                    IsFirstSignIn = true
                };
            }

            return new SignInOnePasswordCommand
            {
                Organization = _environmentAccessor.GetOrganization()
            };
        }

        void UpdateEnvironmentVariables(AppyOnePasswordSession session)
        {
            _environmentAccessor.SetOrganization(session.Organization);
            _environmentAccessor.SetVault(session.Vault);
            _environmentAccessor.SetExecutionEnvironment(session.Environment);
            _environmentAccessor.SetSessionToken(session.SessionToken);
        }

        public async Task<int> ExecuteAsync(params string[] args)
        {
            var argsFixed = new List<string>();
            var argValues = new List<string>();
            foreach (var arg in args)
            {
                if (!arg.StartsWith('-'))
                {
                    argValues.Add(arg);
                    continue;
                }

                if (argValues.Count > 0)
                {
                    argsFixed.Add($"\"{string.Join(" ", argValues)}\"");
                    argValues.Clear();
                }

                argsFixed.Add(arg);
            }

            if (argValues.Count > 0)
            {
                argsFixed.Add($"\"{string.Join(" ", argValues)}\"");
                argValues.Clear();
            }

            var app = new CommandLineApplication
            {
                Name = "appy-op",
                FullName = "Appy 1Password Session Tool"
            };

            app.HelpOption("-h|--help");
            app.VersionOption("-v|--version", GetAssemblyVersion());

            var signInOption = app.Option("-s|--signin", "Signin to 1Password account (eg: \"<organization> <email_address> <secret_key>\")", CommandOptionType.SingleValue);
            var vaultOption = app.Option("-vt|--vault", "1Password vault to use. If not specified, it will use the last known.", CommandOptionType.SingleValue);
            var envOption = app.Option("-env|--environment", "1Password note section environment. If not specified, it will use the last known.", CommandOptionType.SingleValue);
            //var autoRenew = app.Option("-auto|--auto-renew", "Auto renew 1Password session after 30 minutes.", CommandOptionType.NoValue);

            app.OnExecuteAsync(async cancellationToken =>
            {
                var signInCommand = BuildSignInCommand(app, signInOption);
                var signInResult = await _onePasswordTool.Execute(signInCommand);

                var environment = envOption.HasValue() ? envOption.Value() : _environmentAccessor.GetExecutionEnvironment();
                var vault = vaultOption.HasValue() ? vaultOption.Value() : _environmentAccessor.GetVault();

                var session = AppyOnePasswordSession.New(
                    organization: signInCommand.Organization,
                    vault: vault,
                    environment: environment,
                    sessionToken: signInResult.SessionToken);

                UpdateEnvironmentVariables(session);

                _logger.LogInformation($"Appy 1Password Session Started:");
                _logger.LogInformation($"\tOrganization: {session.Organization}");
                _logger.LogInformation($"\tVault: {session.Vault}");
                _logger.LogInformation($"\tEnvironment: {session.Environment}");
                _logger.LogInformation($"\tSessionToken: {session.SessionToken}");

                return 0;
            });

            try
            {
                return await app.ExecuteAsync(argsFixed.ToArray());
            }
            catch (CommandParsingException ex)
            {
                var p = ex.Command;
                _logger.LogError(ex.Message);
                app.ShowHelp();
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return 1;
            }
        }
    }
}