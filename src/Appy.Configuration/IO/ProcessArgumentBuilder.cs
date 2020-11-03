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

        public ProcessArgumentBuilder Append(string argument)
        {
            _tokens.Add(argument);
            return this;
        }

        public ProcessArgumentBuilder AppendIf(bool condition, string argument)
        {
            if (!condition)
            {
                return this;
            }
            
            _tokens.Add(argument);
            return this;
        }

        public ProcessArgumentBuilder Prepend(string argument)
        {
            _tokens.Insert(0, argument);
            return this;
        }

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