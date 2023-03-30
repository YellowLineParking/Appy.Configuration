namespace Appy.Configuration.Common;

public static class ObjectExtensions
{
    public static bool IsNull(this object source) => ReferenceEquals(source, null);
}