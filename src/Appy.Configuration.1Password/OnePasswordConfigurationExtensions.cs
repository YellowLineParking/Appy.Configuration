using System;
using Appy.Configuration.OnePassword.Internals;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.Infrastructure.OnePassword.Tooling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Configuration.OnePassword;

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
    /// <param name="userId">1Password user id</param>
    /// <param name="vault">1Password organization vault</param>
    /// <param name="environment">Execution environment to load from the 1Password Note sections.</param>
    /// <param name="sessionToken">1Password Session Token. The session will last 30 min after you signin with the command line tool.</param>
    /// <param name="configureSource">Configures the source.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder Add1Password(
        this IConfigurationBuilder builder,
        string appName,
        string userId,
        string vault,
        string environment,
        string sessionToken,
        Action<OnePasswordConfigurationSource>? configureSource = null)
    {
        var serviceProvider = OnePasswordConfigurationFactory.BuildServiceProvider();

        var tool = serviceProvider.GetService<IOnePasswordTool>();

        var source = new OnePasswordConfigurationSource(
            tool,
            appName,
            userId,
            vault,
            environment,
            sessionToken);

        configureSource?.Invoke(source);

        return builder.Add(source);
    }

    /// <summary>
    /// Adds 1Password configuration provider for <paramref name="appName"/> to <paramref name="builder"/>.
    /// A series of conventions are followed in coordination with the Appy 1Password console tool:
    /// - AppName: Identifies the application where to load the settings. A secure note is expected to exist
    /// in 1Password with a name of the form "appName.AppSettings". Inside this there should be a section
    /// for each environment with the settings. Each setting should be like eg: "Database:ConnectionString".
    /// - UserId: The user id in the 1Password organization that was previously configured with the console tool.
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
        var serviceProvider = OnePasswordConfigurationFactory.BuildServiceProvider();

        var sessionStorage = serviceProvider.GetService<IOnePasswordSessionStorage>();

        var session = sessionStorage.GetCurrent().GetAwaiter().GetResult();

        return builder.Add1Password(
            appName: appName,
            userId: session.UserId!,
            vault: session.Vault!,
            environment: session.Environment!,
            sessionToken: session.SessionToken!,
            configureSource);
    }
}