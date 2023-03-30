namespace Appy.Configuration.Common;

public interface IPlatformInformation
{
    bool IsRunningOnWindows();
    bool IsRunningOnMacOS();
    bool IsRunningOnLinux();
}