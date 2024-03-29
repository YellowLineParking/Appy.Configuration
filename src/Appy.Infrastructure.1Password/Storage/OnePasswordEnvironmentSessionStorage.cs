using System;
using System.Threading.Tasks;
using Appy.Configuration.Common;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Storage;

public class OnePasswordEnvironmentSessionStorage: IOnePasswordSessionStorage
{
    readonly IPlatformInformation _platformInfo;
    readonly IEnvironmentAccessor _envAccessor;

    public OnePasswordEnvironmentSessionStorage(
        IPlatformInformation platformInfo,
        IEnvironmentAccessor envAccessor)
    {
        _platformInfo = platformInfo ?? throw new ArgumentNullException(nameof(platformInfo));
        _envAccessor = envAccessor ?? throw new ArgumentNullException(nameof(envAccessor));
    }

    public Task<AppyOnePasswordSession> GetCurrent()
    {
        if (_platformInfo.IsRunningOnWindows())
        {
            return Task.FromResult(AppyOnePasswordSession.New(
                organization: _envAccessor.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordOrganization),
                userId: _envAccessor.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordUserId),
                environment: _envAccessor.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordEnvironment),
                sessionToken: _envAccessor.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordSessionToken),
                vault: _envAccessor.GetUserEnvironmentVariable(KnownSessionVars.OnePasswordVault)
            ));
        }

        return Task.FromResult(AppyOnePasswordSession.New(
            organization: _envAccessor.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordOrganization),
            userId: _envAccessor.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordUserId),
            environment: _envAccessor.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordEnvironment),
            sessionToken: _envAccessor.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordSessionToken),
            vault: _envAccessor.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordVault)
        ));
    }

    public Task Update(AppyOnePasswordSession session)
    {
        if (!_platformInfo.IsRunningOnWindows())
        {
            throw new PlatformNotSupportedException("Updating a session using environment variables is not supported on your OS.");
        }

        if (session == null) throw new ArgumentException("1Password Session should not be null.");
        if (string.IsNullOrWhiteSpace(session.Organization)) throw new ArgumentException("1Password Organization must be specified.");
        if (string.IsNullOrWhiteSpace(session.UserId)) throw new ArgumentException("1Password User Id must be specified.");
        if (string.IsNullOrWhiteSpace(session.Environment)) throw new ArgumentException("1Password Environment must be specified.");
        if (string.IsNullOrWhiteSpace(session.Vault)) throw new ArgumentException("1Password Vault must be specified.");
        if (string.IsNullOrWhiteSpace(session.SessionToken)) throw new ArgumentException("1Password SessionToken must be specified.");

        _envAccessor.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordOrganization, session.Organization);
        _envAccessor.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordUserId, session.UserId);
        _envAccessor.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordEnvironment, session.Environment);
        _envAccessor.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordVault, session.Vault);
        _envAccessor.SetUserEnvironmentVariable(KnownSessionVars.OnePasswordSessionToken, session.SessionToken);

        return Task.CompletedTask;
    }
}