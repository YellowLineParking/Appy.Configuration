using System;

namespace Appy.Configuration.Logging;

public class EmptyLogger : ILogger
{
    public void LogVerbose(string message) { }

    public void LogInformation(string message, bool important = false) { }

    public void LogWarning(string message) {}

    public void LogError(string message) {}

    public void LogError(Exception exception) {}
}