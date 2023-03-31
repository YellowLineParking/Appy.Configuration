namespace Appy.Infrastructure.OnePassword.Commands;

public class SignInOnePasswordCommand
{
    public bool IsFirstSignIn { get; set; }

    public string? Organization { get; set; }

    public string? UserId { get; set; }

    public string? Email { get; set; }

    public string? SecretKey { get; set; }
}