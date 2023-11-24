using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Appy.Configuration.IO;

public static class TextReaderExtensions
{
    /// <summary>
    /// Write a line to a <see cref="TextReader"/> and return a <see cref="bool"/> indicating if there is more data to read.
    /// </summary>
    public static async Task<bool?> WriteLineAsyncTo(this TextReader reader, StringBuilder builder, char stopChar, int bufferSize = 1024)
    {
        var buffer = new char[bufferSize];
        var anyDataRead = false;

        while (true)
        {
            var numCharsRead = await reader.ReadAsync(buffer, 0, buffer.Length);
            if (numCharsRead <= 0)
            {
                // End of the stream is reached.
                return anyDataRead ? false : null;
            }

            for (var i = 0; i < numCharsRead; i++)
            {
                var c = buffer[i];
                if (c is '\r' or '\n')
                {
                    // Check if the next character is '\n' in case of '\r\n'
                    if (c == '\r' && i + 1 < numCharsRead && buffer[i + 1] == '\n')
                    {
                        i++; // Skip the '\n'
                    }

                    return reader.Peek() != -1;
                }

                if (c == stopChar)
                {
                    return false;
                }

                builder.Append(c);
            }

            anyDataRead = true;
        }
    }
}