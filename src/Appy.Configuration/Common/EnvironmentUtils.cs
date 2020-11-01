using System;

namespace Appy.Configuration.Common
{
    public static class EnvironmentUtils
    {
        static string GetEnvironmentVariable(string environmentVariable, EnvironmentVariableTarget target)
        {
            return (!string.IsNullOrEmpty(environmentVariable)
                ? Environment.GetEnvironmentVariable(environmentVariable, target)
                : environmentVariable)!;
        }

        public static string GetUserEnvironmentVariable(string environmentVariable)
        {
            return GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.User);
        }

        public static string GetProcessEnvironmentVariable(string environmentVariable)
        {
            return GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
        }

        public static void SetUserEnvironmentVariable(string environmentVariable, string? value)
        {
            Environment.SetEnvironmentVariable(environmentVariable, value, EnvironmentVariableTarget.User);
        }
    }
}