namespace Appy.Infrastructure.OnePassword.Commands
{
    public class SignInOnePasswordCommand
    {
        public string? Email { get; set; }

        public string? Organization { get; set; }

        public string? SecretKey { get; set; }

        public bool IsFirstSignIn { get; set; }
    }
}