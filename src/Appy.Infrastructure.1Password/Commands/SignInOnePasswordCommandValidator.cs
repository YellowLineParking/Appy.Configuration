using Appy.Configuration.Validation;

namespace Appy.Infrastructure.OnePassword.Commands
{
    public class SignInOnePasswordCommandValidator: IValidator<SignInOnePasswordCommand>
    {
        public ValidationResult Validate(SignInOnePasswordCommand command)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(command.Organization))
                return result.WithError(nameof(command.Organization).ToLower(), "1Password organization must be specified");

            if (!command.IsFirstSignIn)
                return result;

            if (string.IsNullOrWhiteSpace(command.Email))
                return result.WithError(nameof(command.Email).ToLower(), "1Password email must be specified");

            if (string.IsNullOrWhiteSpace(command.SecretKey))
                return result.WithError(nameof(command.Email).ToLower(), "1Password secret key must be specified");

            return result;
        }
    }
}
