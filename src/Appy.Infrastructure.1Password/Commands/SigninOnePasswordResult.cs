namespace Appy.Infrastructure.OnePassword.Commands;

public class SignInOnePasswordResult
{
    SignInOnePasswordResult(string userId, string sessionToken)
    {
        UserId = userId;
        SessionToken = sessionToken;
    }

    public static SignInOnePasswordResult Create(string userId, string sessionToken) => new(userId, sessionToken);

    public bool Success => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(SessionToken);

    public string UserId { get; }

    public string SessionToken { get; }
}