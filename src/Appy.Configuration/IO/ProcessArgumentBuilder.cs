using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Appy.Configuration.IO
{
    public sealed class ProcessArgumentBuilder : IReadOnlyCollection<string>
    {
        readonly List<string> _tokens;

        public ProcessArgumentBuilder()
        {
            _tokens = new List<string>();
        }

        public int Count => _tokens.Count;

        public void Clear() => _tokens.Clear();

        public void Append(string argument) => _tokens.Add(argument);

        public void Prepend(string argument) => _tokens.Insert(0, argument);

        public string Render() => string.Join(" ", _tokens.Select(t => t));

        public static implicit operator ProcessArgumentBuilder(string value) => FromString(value);

        public static ProcessArgumentBuilder FromString(string value)
        {
            var builder = new ProcessArgumentBuilder();
            builder.Append(value);
            return builder;
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => _tokens.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_tokens).GetEnumerator();
    }
}