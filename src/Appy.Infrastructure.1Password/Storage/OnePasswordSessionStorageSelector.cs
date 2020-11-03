using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Storage
{
    public class OnePasswordSessionStorageSelector: IOnePasswordSessionStorage
    {
        private readonly IOnePasswordSessionStorage _environmentSessionStorage;
        private readonly IOnePasswordSessionStorage _fileSessionStorage;

        public OnePasswordSessionStorageSelector(
            IOnePasswordSessionStorage environmentSessionStorage,
            IOnePasswordSessionStorage fileSessionStorage)
        {
            _environmentSessionStorage = environmentSessionStorage ?? throw new ArgumentNullException(nameof(environmentSessionStorage));
            _fileSessionStorage = fileSessionStorage ?? throw new ArgumentNullException(nameof(fileSessionStorage));
        }

        static bool IsRunningOnWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public async Task<AppyOnePasswordSession> GetCurrent()
        {
            var session = await _environmentSessionStorage.GetCurrent();

            if (!IsRunningOnWindows && string.IsNullOrWhiteSpace(session.Organization))
            {
                session = await _fileSessionStorage.GetCurrent();
            }

            return session;
        }

        public Task Update(AppyOnePasswordSession session)
        {
            if (IsRunningOnWindows)
            {
                return _environmentSessionStorage.Update(session);
            }

            return _fileSessionStorage.Update(session);
        }
    }
}