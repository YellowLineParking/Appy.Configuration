using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.Logging;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Tool.OnePassword.Logging;
using McMaster.Extensions.CommandLineUtils;

namespace Appy.Tool.OnePassword.CLI
{
    internal class AppyOnePasswordToolCLI : IAppyOnePasswordToolCLI
    {
        readonly ILogger _logger;
        readonly IConsoleVisualzer _consoleVisualizer;
        readonly IOnePasswordSessionStorage _sessionStorage;
        readonly IOnePasswordTool _onePasswordTool;
        const int AutoRenewDelayInMins = 29;
        static string GetAssemblyVersion() => typeof(AppyOnePasswordToolCLI).Assembly.GetName().Version.ToString();

        public AppyOnePasswordToolCLI(
            ILogger logger,
            IConsoleVisualzer consoleVisualizer,
            IOnePasswordSessionStorage sessionStorage,
            IOnePasswordTool onePasswordTool
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _consoleVisualizer = consoleVisualizer ?? throw new ArgumentNullException(nameof(consoleVisualizer));
            _sessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
            _onePasswordTool = onePasswordTool ?? throw new ArgumentNullException(nameof(onePasswordTool));
        }

        SignInOnePasswordCommand BuildSignInCommand(CommandLineApplication app, CommandOption option, AppyOnePasswordSession currentSession)
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
                Organization = currentSession.Organization
            };
        }

        async Task AutoRenewSessionActivity(AppyOnePasswordSession session, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Your session activity will be auto renewed in {AutoRenewDelayInMins} min.");

                await Task.Delay(TimeSpan.FromMinutes(AutoRenewDelayInMins));

                var query = new GetOnePasswordVaultsQuery
                {
                    Organization = session.Organization!,
                    SessionToken = session.SessionToken!
                };

                _logger.LogInformation($"Self-renewing the session activity after {AutoRenewDelayInMins} min.");

                await _onePasswordTool.Execute(query, cancellationToken);

                _logger.LogInformation($"Session activity automatically renewed after {AutoRenewDelayInMins} min.");
            }
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
            var autoRenew = app.Option("-a|--auto-renew", "Automatically renew 1Password session activity before the 30 min expire.", CommandOptionType.NoValue);

            app.OnExecuteAsync(async cancellationToken =>
            {
                var currentSession = await _sessionStorage.GetCurrent();

                var signInCommand = BuildSignInCommand(app, signInOption, currentSession);
                var signInResult = await _onePasswordTool.Execute(signInCommand, cancellationToken);

                var environment = envOption.HasValue() ? envOption.Value() : currentSession.Environment;
                var vault = vaultOption.HasValue() ? vaultOption.Value() : currentSession.Vault;

                var session = AppyOnePasswordSession.New(
                    organization: signInCommand.Organization,
                    vault: vault,
                    environment: environment,
                    sessionToken: signInResult.SessionToken);

                _logger.LogInformation("Updating 1Password session information.");

                await _sessionStorage.Update(session);

                _consoleVisualizer.Render(session);

                if (!autoRenew.HasValue())
                {
                    return 0;
                }

                var foreverCancellationTokenSource = new CancellationTokenSource();

                await AutoRenewSessionActivity(session, foreverCancellationTokenSource.Token);

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