using System;
using Appy.Infrastructure.OnePassword.Tooling;
using Microsoft.Extensions.Configuration;

namespace Appy.Configuration.OnePassword;

/// <summary>
/// Represents 1Password Configuration as <see cref="IConfigurationSource"/>
/// </summary>
public class OnePasswordConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// Creates <see cref="IConfigurationSource"/> for specified <paramref name="appName"/> from 1Password Note.
    /// </summary>
    /// <param name="tool">The <see cref="IOnePasswordTool"/> to use.</param>
    /// <param name="appName">App name pre-configured on 1Password like "appName.AppSettings" and as a secure note</param>
    /// <param name="userId">1Password user id</param>
    /// <param name="vault">1Password organization vault</param>
    /// <param name="environment">Execution environment to load from the 1Password note section.</param>
    /// <param name="sessionToken">1Password Session Token. The session will last 30 min after you signin with the command line tool.</param>
    public OnePasswordConfigurationSource(
        IOnePasswordTool tool,
        string appName,
        string userId,
        string vault,
        string environment,
        string sessionToken)
    {
        Tool = tool ?? throw new ArgumentNullException(nameof(tool));

        if (string.IsNullOrWhiteSpace(appName)) throw new ArgumentException("1Password App name must be specified", nameof(appName));

        AppName = appName;
        UserId = userId;
        Vault = vault;
        Environment = environment;
        SessionToken = sessionToken;
    }

    ///<inheritdoc cref="IOnePasswordTool"/>
    public IOnePasswordTool Tool { get; }

    /// <summary>
    /// Gets App name already pre-configured on 1Password like "AppName.AppSettings"
    /// and as a secure note on the organization vault.
    /// </summary>
    public string AppName { get; }

    /// <summary>
    /// 1Password user id
    /// </summary>
    public string UserId { get; }

    /// <summary>
    /// 1Password organization vault (eg: Development)
    /// </summary>
    public string Vault { get; set; }

    /// <summary>
    /// Execution environment to load from the 1Password note section.
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// 1Password Session Token. The session will last 30 min after you signin with the command line tool.
    /// </summary>
    public string SessionToken { get; set; }

    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new OnePasswordConfigurationProvider(this);
}