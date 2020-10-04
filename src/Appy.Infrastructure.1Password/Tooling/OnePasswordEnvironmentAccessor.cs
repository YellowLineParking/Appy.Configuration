using System;
using System.Collections.Generic;
using System.Linq;
using static Appy.Configuration.Common.EnvironmentUtils;
using static Appy.Infrastructure.OnePassword.Tooling.KnownUserEnvVars;

namespace Appy.Infrastructure.OnePassword.Tooling
{
    public class OnePasswordUserEnvironmentAccessor : IOnePasswordUserEnvironmentAccessor
    {
        static readonly IReadOnlyList<string> AllowedEnvironments = new string[] {"DEV", "QA", "LIVE"};
        static readonly string AllowedEnvironmentsFormatted = string.Join(" ,", AllowedEnvironments);

        public string GetOrganization() => GetUserEnvironmentVariable(OnePasswordOrganization);

        public string GetExecutionEnvironment() => GetUserEnvironmentVariable(OnePasswordEnvironment);

        public string GetSessionToken() => GetUserEnvironmentVariable(OnePasswordSessionToken);

        public string GetVault() => GetUserEnvironmentVariable(OnePasswordVault);

        public void SetOrganization(string organization)
        {
            if (string.IsNullOrWhiteSpace(organization)) throw new ArgumentException("1Password Organization must be specified", nameof(organization));

            SetUserEnvironmentVariable(OnePasswordOrganization, organization);
        }

        public void SetExecutionEnvironment(string? environment)
        {
            if (string.IsNullOrWhiteSpace(environment)) throw new ArgumentException("1Password Execution Environment must be specified", nameof(environment));
            if (!AllowedEnvironments.Contains(environment)) throw new ArgumentException($"1Password Execution Environment should be valid (eg: {AllowedEnvironmentsFormatted})", nameof(environment));
            SetUserEnvironmentVariable(OnePasswordEnvironment, environment);
        }

        public void SetVault(string vault)
        {
            if (string.IsNullOrWhiteSpace(vault)) throw new ArgumentException("1Password Vault must be specified", nameof(vault));

            SetUserEnvironmentVariable(OnePasswordVault, vault);
        }

        public void SetSessionToken(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken)) throw new ArgumentException("1Password SessionToken must be specified", nameof(sessionToken));

            SetUserEnvironmentVariable(OnePasswordSessionToken, sessionToken);
        }
    }
}