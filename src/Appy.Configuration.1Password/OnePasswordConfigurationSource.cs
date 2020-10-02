using System;
using Appy.Infrastructure.OnePassword.Tooling;
using Microsoft.Extensions.Configuration;

namespace Appy.Configuration.OnePassword
{
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
        /// <param name="organization">1Password organization name</param>
        /// <param name="vault">1Password organization vault</param>
        /// <param name="environment">Execution environment to load from the 1Password note section.</param>
        /// <param name="sessionToken">1Password Session Token. The session will last 30 min after you signin with the command line tool.</param>
        public OnePasswordConfigurationSource(
            IOnePasswordTool tool,
            string appName,
            string organization,
            string vault,
            string environment,
            string sessionToken)
        {
            Tool = tool ?? throw new ArgumentNullException(nameof(tool));

            if (string.IsNullOrWhiteSpace(appName)) throw new ArgumentException("1Password App name must be specified", nameof(appName));
            if (string.IsNullOrWhiteSpace(organization)) throw new ArgumentException("1Password Organization must be specified", nameof(organization));
            if (string.IsNullOrWhiteSpace(vault)) throw new ArgumentException("1Password Vault must be specified", nameof(vault));
            if (string.IsNullOrWhiteSpace(environment)) throw new ArgumentException("1Password Environment must be specified", nameof(environment));
            if (string.IsNullOrWhiteSpace(sessionToken)) throw new ArgumentException("1Password SessionToken must be specified", nameof(sessionToken));

            AppName = appName;
            AppSettingsName = $"{appName}.AppSettings";
            Organization = organization;
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
        /// Gets App settings name already pre-configured on 1Password like "AppName.AppSettings"
        /// and as a secure note on the organization vault.
        /// </summary>
        public string AppSettingsName { get; }

        /// <summary>
        /// 1Password organization name (eg: https://yourorg.1password.com/)
        /// </summary>
        public string Organization { get; }

        /// <summary>
        /// 1Password organization vault (eg: Development)
        /// </summary>
        public string Vault { get; }

        /// <summary>
        /// Execution environment to load from the 1Password note section.
        /// </summary>
        public string Environment { get; }

        /// <summary>
        /// 1Password Session Token. The session will last 30 min after you signin with the command line tool.
        /// </summary>
        public string SessionToken { get; }

        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new OnePasswordConfigurationProvider(this);
    }
}
