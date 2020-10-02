namespace Appy.Configuration.IO
{
    public sealed class ProcessResult
    {
        internal ProcessResult(string standardOutput, string standardError, int exitCode)
        {
            StandardOutput = standardOutput;
            StandardError = standardError;
            ExitCode = exitCode;
        }

        public static ProcessResult Create(string standardOutput, string standardError, int exitCode) =>
            new ProcessResult(standardOutput, standardError, exitCode);

        public int ExitCode { get; }

        public bool Success => this.ExitCode == 0;

        public string StandardOutput { get; }

        public string StandardError { get; }
    }
}