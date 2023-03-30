using System;

namespace Appy.Configuration.Common;

public static class EnvironmentUtils
{
    public static string GetEnvironmentVariable(string environmentVariable, EnvironmentVariableTarget target)
    {
        return (!string.IsNullOrEmpty(environmentVariable)
            ? Environment.GetEnvironmentVariable(environmentVariable, target)
            : environmentVariable)!;
    }

    public static string GetProcessEnvironmentVariable(string environmentVariable)
    {
        return EnvironmentUtils.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
    }
}