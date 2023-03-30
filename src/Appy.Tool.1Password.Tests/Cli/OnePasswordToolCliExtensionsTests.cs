using System.Collections.Generic;
using Appy.Tool.OnePassword.Cli;
using FluentAssertions;
using Xunit;

namespace Appy.Tool.OnePassword.Tests.Cli;

public static class OnePasswordToolCliExtensionsTests
{
    public class EscapeArgs
    {
        [Theory]
        [MemberData(nameof(ArgTestCases))]
        public void ShouldEscapeArgs(string[] args, IList<string> expected)
        {
            var result = args.EscapeArgs();
            result.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> ArgTestCases => new List<object[]>
        {
            new object[] { new[] { "-env", "DEV" }, new List<string> { "-env:DEV" } },
            new object[] { new[] { "-s", "appy", "test@appy.com", "TestKey" }, new List<string> { "-s:\"appy test@appy.com TestKey\"" } },
            new object[] { new[] { "--signin", "appy", "test@appy.com", "TestKey" }, new List<string> { "--signin:\"appy test@appy.com TestKey\"" } },
            new object[] { new[] { "-s", "appy", "test@appy.com", "TestKey" }, new List<string> { "-s:\"appy test@appy.com TestKey\"" } },
            new object[] { new[] { "-s", "appy", "test@appy.com", "TestKey", "-vt", "Development", "-env", "DEV", "-api", "6000", "-a" },
                new List<string> { "-s:\"appy test@appy.com TestKey\"", "-vt:Development", "-env:DEV", "-api:6000", "-a" } }
        };
    }

    public class WhenSplitBySpaceAndTrimSpaces
    {
        [Theory]
        [InlineData("appy test@appy.com testKey")]
        [InlineData("appy  test@appy.com   testKey  ")]
        [InlineData("\"appy\"   \"test@appy.com\"   \"testKey\"")]
        [InlineData("\"appy   test@appy.com   testKey\"")]
        public void ShouldReturnValidSplittedValuesWithoutSpacesAndTrimmed(string value)
        {
            var result = value.SplitBySpaceAndTrimSpaces();

            var expected = new List<string> {"appy", "test@appy.com", "testKey"};

            result.Should().BeEquivalentTo(expected);
        }
    }
}