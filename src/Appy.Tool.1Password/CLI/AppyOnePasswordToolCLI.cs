using System;
using System.Linq;
using System.Threading.Tasks;
using Appy.Configuration.Logging;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Tool.OnePassword.Logging;
using McMaster.Extensions.CommandLineUtils;

namespace Appy.Tool.OnePassword.CLI
{
    internal class AppyOnePasswordToolCLI : IAppyOnePasswordToolCLI
    {
        readonly ILogger _logger;
        readonly IConsoleVisualzer _consoleVisualizer;
        readonly IOnePasswordUserEnvironmentAccessor _environmentAccessor;
        readonly IOnePasswordTool _onePasswordTool;
        static string GetAssemblyVersion() => typeof(AppyOnePasswordToolCLI).Assembly.GetName().Version.ToString();
        public AppyOnePasswordToolCLI(
            ILogger logger,
            IConsoleVisualzer consoleVisualizer,
            IOnePasswordUserEnvironmentAccessor environmentAccessor,
            IOnePasswordTool onePasswordTool
        )
        {
            _logger = logger;
            _consoleVisualizer = consoleVisualizer;
            _environmentAccessor = environmentAccessor;
            _onePasswordTool = onePasswordTool;
        }
       
        SignInOnePasswordCommand BuildSignInCommand(CommandLineApplication app, CommandOption option)
        {
            if (!option.HasValue())
            {
                throw new CommandParsingException(app, "Signin option must be specified.");
            }

            var parameters = option.Value()!.SplitBySpaceAndTrimSpaces();

            var hasValidParameters = parameters.Count == 0 || parameters.Count == 3;
            if (!hasValidParameters)
            {
                throw new CommandParsingException(app, "Signin values must be specified: <organization> <email_address> <secret_key>.");
            }

            if (parameters.Count > 0)
            {
                return new SignInOnePasswordCommand
                {
                    Organization = parameters.ElementAt(0),
                    Email = parameters.ElementAt(1),
                    SecretKey = parameters.ElementAt(2),
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
            var adaptedArgs = AppyOnePasswordTooExtensions.EscapeArgs(args);

            var app = new CommandLineApplication
            {
                Name = "appy-op",
                FullName = "Appy 1Password Session Tool"
            };

            app.HelpOption("-h|--help");
            app.VersionOption("-v|--version", GetAssemblyVersion());

            var signInOption = app.Option("-s|--signin", "Signin to 1Password account (eg: --signin <organization> <email_address> <secret_key>)", CommandOptionType.SingleOrNoValue);
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

                _consoleVisualizer.Render(session);

                return 0;
            });

            try
            {
                return await app.ExecuteAsync(adaptedArgs.ToArray());
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