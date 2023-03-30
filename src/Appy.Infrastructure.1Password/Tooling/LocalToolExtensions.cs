using System;
using Appy.Configuration.IO;
using Appy.Configuration.Serializers;

namespace Appy.Infrastructure.OnePassword.Tooling;

internal static class LocalToolExtensions
{
    internal static void ValidateAndThrow(this ProcessResult result)
    {
        if (!result.Success)
        {
            throw new OnePasswordToolException($"1Password internal CLI failed with exit code {result.ExitCode}: {result.StandardError}");
        }
    }

    internal static TResult DeserializeAndThrow<TResult>(this IAppyJsonSerializer serializer, string output)
    {
        try
        {
            return serializer.Deserialize<TResult>(output);
        }
        catch (Exception ex)
        {
            throw new OnePasswordToolException("1Password Tool failed deserialization", ex);
        }
    }
}