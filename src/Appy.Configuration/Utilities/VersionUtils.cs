using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Appy.Configuration.Utilities
{
    public static class VersionUtilities
    {
        public static string GetVersion(Assembly assembly)
        {
            if (assembly == null)
            {
                return "?";
            }

            try
            {
                var info = FileVersionInfo.GetVersionInfo(assembly.Location);
                return info.ProductVersion?.Split('+').FirstOrDefault() ?? "?";
            }
            catch
            {
                return "?";
            }
        }
    }
}