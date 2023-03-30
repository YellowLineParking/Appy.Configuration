using System;
using System.IO;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Xunit.Abstractions;

namespace Appy.Tool.OnePassword.Tests.Cli.Utils;

/// <summary>
/// Via: https://github.com/natemcmaster/CommandLineUtils/blob/main/test/CommandLineUtils.Tests/Utilities/TestConsole.cs
/// </summary>
public class TestConsole : IConsole
{
    public TestConsole(ITestOutputHelper output)
    {
        Out = new XUnitTextWriter(output);
        Error = new XUnitTextWriter(output);
    }

    public TextWriter Out { get; set; }
    public TextWriter Error { get; set; }

    public TextReader In => throw new NotImplementedException();

    public bool IsInputRedirected => throw new NotImplementedException();

    public bool IsOutputRedirected => true;

    public bool IsErrorRedirected => true;

    public ConsoleColor ForegroundColor { get; set; }
    public ConsoleColor BackgroundColor { get; set; }

    public event ConsoleCancelEventHandler CancelKeyPress;

    public void ResetColor()
    {
    }

    public void RaiseCancelKeyPress()
    {
        // See https://github.com/dotnet/corefx/blob/f2292af3a1794378339d6f5c8adcc0f2019a2cf9/src/System.Console/src/System/ConsoleCancelEventArgs.cs#L14
        var eventArgs = typeof(ConsoleCancelEventArgs)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .First()
            .Invoke(new object[] { ConsoleSpecialKey.ControlC });
        CancelKeyPress?.Invoke(this, (ConsoleCancelEventArgs)eventArgs);
    }
}