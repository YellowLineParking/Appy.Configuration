namespace Appy.Infrastructure.OnePassword.Commands
{
    public class SignInOnePasswordResult
    {
        protected SignInOnePasswordResult(string sessionToken)
        {
            SessionToken = sessionToken;
        }

        public static SignInOnePasswordResult Create(string sessionToken) =>
            new SignInOnePasswordResult(sessionToken);

        public bool Success => !string.IsNullOrWhiteSpace(SessionToken);
        public string SessionToken { get; }
    }
}