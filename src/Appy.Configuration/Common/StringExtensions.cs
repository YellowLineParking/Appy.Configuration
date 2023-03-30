namespace Appy.Configuration.Common;

public static class StringExtensions
{
    public static int? ToInt32(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return int.Parse(value);
    }
}