using System;
using System.Collections.Generic;
using System.Linq;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Tooling;

namespace Appy.Tool.OnePassword.CLI
{
    internal static class AppyOnePasswordTooExtensions
    {
        internal static IEnumerable<string> EscapeArgs(params string[] args)
        {
            if (args.Length == 0) return args;

            var argsFixed = new List<string>();
            var argValues = new List<string>();

            foreach (var arg in args)
            {
                if (!arg.StartsWith('-'))
                {
                    argValues.Add(arg);
                    continue;
                }

                if (argValues.Count == 1)
                {
                    argsFixed.Add(argValues.First());
                    argValues.Clear();
                }

                if (argValues.Count > 1)
                {
                    argsFixed[^1] = $"{argsFixed[^1]}:\"{string.Join(" ", argValues)}\"";
                    argValues.Clear();
                }

                argsFixed.Add(arg);
            }

            if (argValues.Count == 1)
            {
                argsFixed.Add(argValues.First());
                argValues.Clear();
            }

            if (argValues.Count > 1)
            {
                argsFixed[^1] = $"{argsFixed[^1]}:\"{string.Join(" ", argValues)}\"";
                argValues.Clear();
            }

            return argsFixed;
        }

        internal static IList<string> SplitBySpaceAndTrimSpaces(this string value)
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