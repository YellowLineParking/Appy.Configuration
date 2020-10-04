using System;

namespace Appy.Tool.OnePassword.Logging
{
    internal interface ILogger
    {
        void LogVerbose(string message);
        void LogInformation(string message, bool important = false);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(Exception exception);
    }
}