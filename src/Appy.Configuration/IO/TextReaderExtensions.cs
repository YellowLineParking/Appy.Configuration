using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Appy.Configuration.IO;

public static class TextReaderExtensions
{
    public static async Task<StreamResult?> ReadLineAsync(this TextReader reader, StringBuilder builder)
    {
        var written = false;
        var chars = new char[1];
        while (true)
        {
            var num = await reader.ReadAsync(chars, 0, chars.Length);
            if (num <= 0)
            {
                return written ? new StreamResult(builder.ToString(), hasMore: reader.Peek() != -1) : null;
            }

            if (chars[0] == '\r' || chars[0] == '\n')
            {
                builder.Append(chars[0]);
                if (chars[0] == '\r' && reader.Peek() == (int)'\n')
                {
                    builder.Append((char)reader.Read());
                }

                return new StreamResult(builder.ToString(), hasMore: reader.Peek() != -1);
            }

            written = true;
            builder.Append(chars[0]);

            // to anticipate last non-ending line
            if (reader.Peek() == -1)
            {
                return written ? new StreamResult(builder.ToString(), hasMore: reader.Peek() != -1) : null;
            }
        }
    }
}