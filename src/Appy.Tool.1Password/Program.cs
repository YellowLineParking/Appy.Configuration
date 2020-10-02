using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword
{
    class Program
    {
        static int Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            //serviceCollection.AddTransient<IRetryHelper, RetryHelper>();
            //serviceCollection.AddTransient<ILogger, ConsoleLogger>();
            
            //serviceCollection.AddSingleton<ISourceRootTranslator, SourceRootTranslator>(provider => new SourceRootTranslator(provider.GetRequiredService<ILogger>(), provider.GetRequiredService<IFileSystem>()));

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            //var logger = (ConsoleLogger)serviceProvider.GetService<ILogger>();
            //var fileSystem = serviceProvider.GetService<IFileSystem>();

            var exitCode = 0;

            var app = new CommandLineApplication();
            app.Name = "Appy 1Password Tool";
            app.FullName = "Appy 1Password Session Manager Tool";
            app.HelpOption("-h|--help");
            app.VersionOption("-v|--version", GetAssemblyVersion());

            //int exitCode = (int)CommandExitCodes.Success;

            //CommandArgument module = app.Argument("<ASSEMBLY>", "Path to the test assembly.");

            var signin = app.Option("-s|--signin", "Signin to 1Password account and set Appy session user environment vars <organization_name> <email_address> <secret_key>", CommandOptionType.MultipleValue);
            var vault = app.Option("-v|--vault", "1Password Vault to use (if not specified, it will use the last known.)", CommandOptionType.SingleValue);
            var environment = app.Option("-env|--environment", "Section environment to load settings.", CommandOptionType.MultipleValue);
            var autoRenew = app.Option("-auto|--auto-renew", "Auto renew 1Password session after 30 minutes.", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                //if (string.IsNullOrEmpty(module.Value) || string.IsNullOrWhiteSpace(module.Value))
                //    throw new CommandParsingException(app, "No test assembly specified.");

                if (!signin.HasValue())
                    throw new CommandParsingException(app, "Target must be specified.");

                //if (verbosity.HasValue())
                //{
                //    // Adjust log level based on user input.
                //    logger.Level = verbosity.ParsedValue;
                //}

                // logger.LogInformation("  Outputting results to console", important: true);

                var exitCode = 0;

                return exitCode;
            });

            try
            {
                return app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                var p = ex.Command;
                //logger.LogError(ex.Message);
                app.ShowHelp();
                // return (int)CommandExitCodes.CommandParsingException;
            }
            //catch (Win32Exception we) when (we.Source == "System.Diagnostics.Process")
            //{
            //    logger.LogError($"Start process '{target.Value()}' failed with '{we.Message}'");
            //    return exitCode > 0 ? exitCode : (int)CommandExitCodes.Exception;
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex.Message);
            //    return exitCode > 0 ? exitCode : (int)CommandExitCodes.Exception;
            //}

            return exitCode;
        }

        static string GetAssemblyVersion() => typeof(Program).Assembly.GetName().Version.ToString();
    }
}