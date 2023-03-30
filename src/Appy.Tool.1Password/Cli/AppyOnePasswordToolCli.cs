using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.Logging;
using Appy.Configuration.Utilities;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Tool.OnePassword.Api;
using Appy.Tool.OnePassword.Logging;
using McMaster.Extensions.CommandLineUtils;
using Appy.Configuration.Scheduling;
using Appy.Configuration.Validation;

namespace Appy.Tool.OnePassword.Cli;

public class AppyOnePasswordToolCli : IAppyOnePasswordToolCli
{
    readonly ILogger _logger;
    readonly ICommandLineApplicationFactory _commandLineAppFactory;
    readonly IConsoleVisualzer _consoleVisualizer;
    readonly IOnePasswordApiRunner _apiRunner;
    readonly IOnePasswordSessionStorage _sessionStorage;
    readonly IJobScheduler _toolScheduler;
    readonly IOnePasswordTool _onePasswordTool;
    const int AutoRenewDelayInMins = 29;
    static string GetVersion() => VersionUtilities.GetVersion(typeof(AppyOnePasswordToolCli).Assembly);

    public AppyOnePasswordToolCli(
        ILogger logger,
        ICommandLineApplicationFactory commandLineAppFactory,
        IConsoleVisualzer consoleVisualizer,
        IOnePasswordApiRunner apiRunner,
        IOnePasswordSessionStorage sessionStorage,
        IJobScheduler toolScheduler,
        IOnePasswordTool onePasswordTool
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandLineAppFactory = commandLineAppFactory ?? throw new ArgumentNullException(nameof(commandLineAppFactory));
        _consoleVisualizer = consoleVisualizer ?? throw new ArgumentNullException(nameof(consoleVisualizer));
        _apiRunner = apiRunner ?? throw new ArgumentNullException(nameof(apiRunner));
        _sessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
        _toolScheduler = toolScheduler ?? throw new ArgumentNullException(nameof(toolScheduler));
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

    async Task RenewSessionActivity(AppyOnePasswordSession session, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Self-renewing the session activity after {AutoRenewDelayInMins} min.");

        var query = new GetOnePasswordVaultsQuery
        {
            Organization = session.Organization!,
            SessionToken = session.SessionToken!
        };

        await _onePasswordTool.Execute(query, cancellationToken);

        _logger.LogInformation($"Session activity automatically renewed after {AutoRenewDelayInMins} min.");

        _logger.LogInformation($"Your session activity will be auto renewed in {AutoRenewDelayInMins} min.");
    }

    async Task AutoRenewSessionActivity(AppyOnePasswordSession session, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Your session activity will be auto renewed in {AutoRenewDelayInMins} min.");

        await _toolScheduler.ScheduleJobAndBlock(
            job: () => RenewSessionActivity(session, cancellationToken),
            interval: TimeSpan.FromMinutes(AutoRenewDelayInMins),
            cancellationToken);
    }

    public async Task<int> ExecuteAsync(params string[] args)
    {
        var adaptedArgs = args.EscapeArgs();

        var app = _commandLineAppFactory.Create(
            name: "appy-op",
            fullName: "Appy 1Password Session Tool"
        );

        app.HelpOption("-h|--help");
        app.VersionOption("-v|--version", GetVersion());

        var signInOption = app.Option("-s|--signin", "Signin to 1Password account (eg: --signin <organization> <email_address> <secret_key>).", CommandOptionType.SingleOrNoValue);
        var vaultOption = app.Option("-vt|--vault", "1Password vault to use. If not specified, it will use the last known.", CommandOptionType.SingleValue);
        var envOption = app.Option("-env|--environment", "1Password note section environment. If not specified, it will use the last known.", CommandOptionType.SingleValue);
        var autoRenew = app.Option("-a|--auto-renew", "Automatically renew 1Password session activity before the 30 min expire.", CommandOptionType.NoValue);
        var apiOption = app.Option<int?>("-api|--local-api", "Launch a local Http API (default port 5500) to get configurations for session (eg: -api 6000).", CommandOptionType.SingleOrNoValue);

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

            _logger.LogInformation(string.Empty);
            _logger.LogInformation("You can now go to your project and start your debug session.");

            if (!autoRenew.HasValue())
            {
                _logger.LogInformation("Session tokens expire after 30 minutes of inactivity, after which you’ll need to sign in again.");
            }

            if (apiOption.HasValue())
            {
                var apiSettings = new OnePasswordApiSettings
                {
                    StartWithoutBlocking = autoRenew.HasValue(),
                    Port = apiOption.ParsedValue ?? 5500
                };

                _logger.LogInformation($"Appy 1Password session API listening on port: {apiSettings.Port}");

                _apiRunner.Start(apiSettings);
            }

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
            _logger.LogError(ex.Message);

            app.ShowHelp();

            return 1;
        }
        catch (ValidationException ex)
        {
            var result = ex.Result?.Errors?.FirstOrDefault()?.Message;

            _logger.LogError(result);

            return 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            return 1;
        }
    }
}