using System;
using Microsoft.Extensions.Configuration;
using static Appy.Configuration.Common.EnvironmentUtils;
using static Appy.Infrastructure.OnePassword.Tooling.KnownUserEnvVars;

namespace Appy.Configuration.OnePassword
{
    /// <summary>
    /// Extension methods for adding <see cref="OnePasswordConfigurationProvider"/>
    /// </summary>
    public static class OnePasswordConfigurationExtensions
    {
        /// <summary>
        /// Adds 1Password configuration provider for <paramref name="appName"/> to <paramref name="builder"/>.
        /// A series of conventions are followed:
        /// - AppName: Identifies the application where to load the settings. A secure note is expected to exist
        /// in 1Password with a name of the form "appName.AppSettings". Inside this there should be a section
        /// for each environment with the settings. Each setting should be like eg: "Database:ConnectionString".
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="appName">App name pre-configured on 1Password like "appName.AppSettings" and as a secure note</param>
        /// <param name="organization">1Password organization name</param>
        /// <param name="vault">1Password organization vault</param>
        /// <param name="environment">Execution environment to load from the 1Password Note sections.</param>
        /// <param name="sessionToken">1Password Session Token. The session will last 30 min after you signin with the command line tool.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder Add1Password(
            this IConfigurationBuilder builder,
            string appName,
            string organization,
            string vault,
            string environment,
            string sessionToken,
            Action<OnePasswordConfigurationSource>? configureSource = null)
        {
            var jsonSerializer = OnePasswordConfigurationFactory.CreateDefaultSerializer();
            var processRunner = OnePasswordConfigurationFactory.CreateDefaultProcessRunner();
            var tool = OnePasswordConfigurationFactory.CreateDefaultOnePasswordTool(jsonSerializer, processRunner);

            var source = new OnePasswordConfigurationSource(
                tool,
                appName,
                organization,
                vault,
                environment,
                sessionToken);

            configureSource?.Invoke(source);

            return builder.Add(source);
        }

        /// <summary>
        /// Adds 1Password configuration provider for <paramref name="appName"/> to <paramref name="builder"/>.
        /// A series of conventions are followed in coordination with the AppyWay 1Password console tool:
        /// - AppName: Identifies the application where to load the settings. A secure note is expected to exist
        /// in 1Password with a name of the form "appName.AppSettings". Inside this there should be a section
        /// for each environment with the settings. Each setting should be like eg: "Database:ConnectionString".
        /// - Organization: The organization in 1Password to use will depend on what was previously configured with the console tool.
        /// - Vault: The vault to use will depend on what was previously configured with the console tool.
        /// - Environment: The environment to be loaded will depend on what was previously configured with the console tool.
        /// - Session Token: The environment to be loaded will depend on what was previously configured with the console tool.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="appName">App name pre-configured on 1Password like "appName.AppSettings" and as a secure note</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder Add1Password(
            this IConfigurationBuilder builder,
            string appName,
            Action<OnePasswordConfigurationSource>? configureSource = null)
        {
            var organization = GetUserEnvironmentVariable(OnePasswordOrganization);
            if (string.IsNullOrWhiteSpace(organization))
                throw new ArgumentException($"1Password Organization should be in the user environment variables like '${OnePasswordOrganization}'");

            var vault = GetUserEnvironmentVariable(OnePasswordVault);
            if (string.IsNullOrWhiteSpace(vault))
                throw new ArgumentException($"1Password Vault should be in the user environment variables like '${OnePasswordVault}'");

            var environment = GetUserEnvironmentVariable(OnePasswordEnvironment);
            if (string.IsNullOrWhiteSpace(environment))
                throw new ArgumentException($"1Password Environment should be in the user environment variables like '${OnePasswordEnvironment}'");

            var sessionToken = GetUserEnvironmentVariable(OnePasswordSessionToken);
            if (string.IsNullOrWhiteSpace(sessionToken))
                throw new ArgumentException($"1Password Session Token should be in user environment variables like '${OnePasswordSessionToken}'");

            return builder.Add1Password(
                appName: appName,
                organization: organization,
                vault: vault,
                environment: environment,
                sessionToken: sessionToken,
                configureSource);
        }
    }
}