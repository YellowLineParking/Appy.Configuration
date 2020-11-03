using System.Runtime.InteropServices;

namespace Appy.Configuration.Common
{
    public class PlatformInformation: IPlatformInformation
    {
        public bool IsRunningOnWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public bool IsRunningOnMacOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public bool IsRunningOnLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
    }
}