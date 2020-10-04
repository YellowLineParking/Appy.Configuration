using System;

namespace Appy.Tool.OnePassword.CLI
{
    public class AppyOnePasswordSession
    {
        private AppyOnePasswordSession(
            string organization,
            string vault,
            string environment,
            string sessionToken)
        {
            Organization = organization;
            Vault = vault;
            Environment = environment;
            SessionToken = sessionToken;
        }

        public static AppyOnePasswordSession New(
            string? organization,
            string? vault,
            string? environment,
            string? sessionToken)
        {
            if (string.IsNullOrWhiteSpace(organization)) throw new ArgumentException("1Password organization must be specified.");
            if (string.IsNullOrWhiteSpace(vault)) throw new ArgumentException( "1Password vault must be specified.");
            if (string.IsNullOrWhiteSpace(environment)) throw new ArgumentException( "1Password section environment must be specified.");
            if (string.IsNullOrWhiteSpace(sessionToken)) throw new ArgumentException( "1Password session token must be specified.");

            return new AppyOnePasswordSession(
                organization: organization,
                vault: vault,
                environment: environment,
                sessionToken: sessionToken);
        }

        public string Organization { get; }
        public string Environment { get; }
        public string Vault { get; }
        public string SessionToken { get; }
    }
}