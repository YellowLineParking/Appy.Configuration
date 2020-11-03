using System;

namespace Appy.Configuration.Common
{
    public class EnvironmentAccessor : IEnvironmentAccessor
    {
        public string GetUserEnvironmentVariable(string environmentVariable)
        {
            return EnvironmentUtils.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.User);
        }

        public string GetProcessEnvironmentVariable(string environmentVariable)
        {
            return EnvironmentUtils.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
        }

        public void SetUserEnvironmentVariable(string environmentVariable, string? value)
        {
            Environment.SetEnvironmentVariable(environmentVariable, value, EnvironmentVariableTarget.User);
        }

        public static void SetProcessEnvironmentVariable(string environmentVariable, string? value)
        {
            Environment.SetEnvironmentVariable(environmentVariable, value, EnvironmentVariableTarget.Process);
        }
    }
}