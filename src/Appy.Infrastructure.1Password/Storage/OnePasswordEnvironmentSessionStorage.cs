using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Appy.Configuration.Common;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Storage
{
    public class OnePasswordEnvironmentSessionStorage: IOnePasswordSessionStorage
    {
        static bool IsRunningOnWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public Task<AppyOnePasswordSession> GetCurrent()
        {
            if (IsRunningOnWindows)
            {
                return Task.FromResult(AppyOnePasswordSession.New(
                    organization: EnvironmentUtils.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordOrganization),
                    environment: EnvironmentUtils.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordEnvironment),
                    sessionToken: EnvironmentUtils.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordSessionToken),
                    vault: EnvironmentUtils.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordVault)
                ));
            }

            return Task.FromResult(AppyOnePasswordSession.New(
                organization: EnvironmentUtils.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordOrganization),
                environment: EnvironmentUtils.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordEnvironment),
                sessionToken: EnvironmentUtils.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordSessionToken),
                vault: EnvironmentUtils.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordVault)
            ));
        }

        public Task Update(AppyOnePasswordSession session)
        {
            if (!IsRunningOnWindows)
            {
                throw new PlatformNotSupportedException("Updating a session using environment variables is not supported on your OS.");
            }

            if (session == null) throw new ArgumentException("1Password Session should not be null", nameof(session));
            if (string.IsNullOrWhiteSpace(session.Organization)) throw new ArgumentException("1Password Organization must be specified", nameof(AppyOnePasswordSession.Organization));
            if (string.IsNullOrWhiteSpace(session.Environment)) throw new ArgumentException("1Password Execution Environment must be specified", nameof(AppyOnePasswordSession.Environment));
            if (string.IsNullOrWhiteSpace(session.SessionToken)) throw new ArgumentException("1Password SessionToken must be specified", nameof(AppyOnePasswordSession.SessionToken));
            if (string.IsNullOrWhiteSpace(session.Vault)) throw new ArgumentException("1Password Vault must be specified", nameof(session.Vault));

            EnvironmentUtils.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordOrganization, session.Organization);
            EnvironmentUtils.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordEnvironment, session.Environment);
            EnvironmentUtils.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordVault, session.Vault);
            EnvironmentUtils.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordSessionToken, session.SessionToken);

            return Task.CompletedTask;
        }
    }
}