using System;

namespace Appy.Configuration.Common
{
    public static class EnvironmentUtils
    {
        public static string GetUserEnvironmentVariable(string environmentVariable) =>
            (!string.IsNullOrEmpty(environmentVariable)
                ? Environment.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.User)
                : environmentVariable)!;

        public static void SetUserEnvironmentVariable(string environmentVariable, string? value) =>
            Environment.SetEnvironmentVariable(environmentVariable, value, EnvironmentVariableTarget.User);
    }
}