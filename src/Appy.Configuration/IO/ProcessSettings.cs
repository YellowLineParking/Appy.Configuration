using System.Collections.Generic;

namespace Appy.Configuration.IO
{
    public sealed class ProcessSettings
    {
        public ProcessArgumentBuilder Arguments { get; set; }

        public string WorkingDirectory { get; set; }

        public IDictionary<string, string> EnvironmentVariables { get; set; }
    }
}