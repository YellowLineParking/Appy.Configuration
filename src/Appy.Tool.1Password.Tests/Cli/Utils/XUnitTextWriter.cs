using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Appy.Tool.OnePassword.Tests.Cli.Utils
{
    /// <summary>
    /// Via: https://github.com/natemcmaster/CommandLineUtils/blob/main/test/CommandLineUtils.Tests/Utilities/XunitTextWriter.cs
    /// </summary>
    public class XUnitTextWriter : TextWriter
    {
        private readonly ITestOutputHelper _output;
        private readonly StringBuilder _sb = new StringBuilder();

        public XUnitTextWriter(ITestOutputHelper output)
        {
            _output = output;
        }

        public override Encoding Encoding => Encoding.Unicode;

        public override void Write(char ch)
        {
            if (_output == null)
            {
                return;
            }

            if (ch == '\n')
            {
                _output.WriteLine(_sb.ToString());
                _sb.Clear();
            }
            else
            {
                _sb.Append(ch);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sb.Length > 0)
                {
                    _output?.WriteLine(_sb.ToString());
                    _sb.Clear();
                }
            }

            base.Dispose(disposing);
        }
    }
}