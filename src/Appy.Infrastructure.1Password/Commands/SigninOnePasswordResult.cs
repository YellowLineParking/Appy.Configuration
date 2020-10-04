namespace Appy.Infrastructure.OnePassword.Commands
{
    public class SigninOnePasswordResult
    {
        protected SigninOnePasswordResult(string sessionToken)
        {
            SessionToken = sessionToken;
        }

        public static SigninOnePasswordResult Create(string sessionToken) =>
            new SigninOnePasswordResult(sessionToken);

        public bool Success => !string.IsNullOrWhiteSpace(SessionToken);
        public string SessionToken { get; }
    }
}