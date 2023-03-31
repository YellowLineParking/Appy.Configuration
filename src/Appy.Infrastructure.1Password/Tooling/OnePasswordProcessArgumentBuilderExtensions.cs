using Appy.Configuration.IO;
using Appy.Infrastructure.OnePassword.Commands;

namespace Appy.Infrastructure.OnePassword.Tooling;

public static class OnePasswordProcessArgumentBuilderExtensions
{
    public static ProcessArgumentBuilder WithAddAccountAndSigninParameters(this ProcessArgumentBuilder builder, SignInOnePasswordCommand command)
    {
        return builder
            .Append("account")
            .Append("add")
            .Append("--address")
            .Append($"https://{command.Organization}.1password.com")
            .Append("--email")
            .Append(command.Email)
            .Append("--secret-key")
            .Append(command.SecretKey)
            .Append("--shorthand")
            .Append(OnePasswordAccountHelper.ShorthandForOrg(command.Organization))
            .Append($"--signin")
            .Append($"--raw");
    }

    public static ProcessArgumentBuilder WithSigninWithAccountParameters(this ProcessArgumentBuilder builder, string organization)
    {
        return builder
            .Append("signin")
            .Append("--account")
            .Append(OnePasswordAccountHelper.ShorthandForOrg(organization))
            .Append("--raw");
    }

    public static ProcessArgumentBuilder WithJsonFormat(this ProcessArgumentBuilder builder)
    {
        return builder
            .Append("--format")
            .Append("json");
    }
}