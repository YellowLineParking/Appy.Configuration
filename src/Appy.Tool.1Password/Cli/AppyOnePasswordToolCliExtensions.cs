using System.Collections.Generic;
using System.Linq;

namespace Appy.Tool.OnePassword.Cli
{
    public static class AppyOnePasswordToolCliExtensions
    {
        static void AdaptByOptionValuesCount(List<string> argValues, List<string> argsFixed)
        {
            if (argValues.Count == 1)
            {
                argsFixed[^1] = $"{argsFixed[^1]}:{string.Join(" ", argValues)}";
                argValues.Clear();
            }

            if (argValues.Count > 0)
            {
                argsFixed[^1] = $"{argsFixed[^1]}:\"{string.Join(" ", argValues)}\"";
                argValues.Clear();
            }
        }

        public static IEnumerable<string> EscapeArgs(this string[] args)
        {
            if (args.Length == 0)
            {
                return args;
            }

            var argsFixed = new List<string>();
            var argValues = new List<string>();

            foreach (var arg in args)
            {
                if (!arg.StartsWith('-'))
                {
                    argValues.Add(arg);
                    continue;
                }

                AdaptByOptionValuesCount(argValues, argsFixed);

                argsFixed.Add(arg);
            }

            AdaptByOptionValuesCount(argValues, argsFixed);

            return argsFixed;
        }

        public static IList<string> SplitBySpaceAndTrimSpaces(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Enumerable.Empty<string>().ToList();
            }

            return value.Split(' ')
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.Trim('\"', ' '))
                .ToList();
        }
    }
}