using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Appy.Configuration.IO;

public sealed class ProcessSettings
{
    public ProcessArgumentBuilder Arguments { get; set; }

    public string WorkingDirectory { get; set; }

    public IDictionary<string, string>? EnvironmentVariables { get; set; }

    public bool RedirectStandardInput { get; set; } = true;

    public bool CreateNoWindow { get; set; } = true;

    public bool UseShellExecute { get; set; }

    public Func<TextReader, Task>? StandardErrorReader { get; set; }
}