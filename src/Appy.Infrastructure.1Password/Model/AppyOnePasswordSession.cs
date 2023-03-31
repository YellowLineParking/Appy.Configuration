namespace Appy.Infrastructure.OnePassword.Model;

public class AppyOnePasswordSession
{
    AppyOnePasswordSession()
    { }

    AppyOnePasswordSession(
        string? organization,
        string? userId,
        string? vault,
        string? environment,
        string? sessionToken)
    {
        Organization = organization;
        UserId = userId;
        Vault = vault;
        Environment = environment;
        SessionToken = sessionToken;
    }

    public string? Organization { get; }
    public string? UserId { get; }
    public string? Environment { get; }
    public string? Vault { get; }
    public string? SessionToken { get; }

    public static AppyOnePasswordSession Empty() => new();

    public static AppyOnePasswordSession New(
        string? organization,
        string? userId,
        string? vault,
        string? environment,
        string? sessionToken)
    {
        return new AppyOnePasswordSession(
            organization: organization,
            userId: userId,
            vault: vault,
            environment: environment,
            sessionToken: sessionToken);
    }
}