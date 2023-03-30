using System;
using System.Threading.Tasks;
using Appy.Configuration.Common;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Storage;

public class OnePasswordSessionStorageSelector: IOnePasswordSessionStorage
{
    readonly IPlatformInformation _platformInfo;
    readonly IOnePasswordSessionStorage _environmentSessionStorage;
    readonly IOnePasswordSessionStorage _fileSessionStorage;

    public OnePasswordSessionStorageSelector(
        IPlatformInformation platformInfo,
        IOnePasswordSessionStorage environmentSessionStorage,
        IOnePasswordSessionStorage fileSessionStorage)
    {
        _platformInfo = platformInfo ?? throw new ArgumentNullException(nameof(platformInfo));
        _environmentSessionStorage = environmentSessionStorage ?? throw new ArgumentNullException(nameof(environmentSessionStorage));
        _fileSessionStorage = fileSessionStorage ?? throw new ArgumentNullException(nameof(fileSessionStorage));
    }

    public async Task<AppyOnePasswordSession> GetCurrent()
    {
        var session = await _environmentSessionStorage.GetCurrent();

        if (!_platformInfo.IsRunningOnWindows() && string.IsNullOrWhiteSpace(session.Organization))
        {
            session = await _fileSessionStorage.GetCurrent();
        }

        return session;
    }

    public Task Update(AppyOnePasswordSession session)
    {
        if (_platformInfo.IsRunningOnWindows())
        {
            return _environmentSessionStorage.Update(session);
        }

        return _fileSessionStorage.Update(session);
    }
}