using System;

namespace Appy.Infrastructure.OnePassword.Model
{
    public class AppyOnePasswordSession
    {
        private AppyOnePasswordSession()
        {
        }

        private AppyOnePasswordSession(
            string? organization,
            string? vault,
            string? environment,
            string? sessionToken)
        {
            Organization = organization;
            Vault = vault;
            Environment = environment;
            SessionToken = sessionToken;
        }

        public string? Organization { get; }
        public string? Environment { get; }
        public string? Vault { get; }
        public string? SessionToken { get; }

        public static AppyOnePasswordSession Empty()
        {
            return new AppyOnePasswordSession();
        }

        public static AppyOnePasswordSession New(
            string? organization,
            string? vault,
            string? environment,
            string? sessionToken)
        {
            return new AppyOnePasswordSession(
                organization: organization,
                vault: vault,
                environment: environment,
                sessionToken: sessionToken);
        }
    }
}