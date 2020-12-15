using Appy.Configuration.Validation;

namespace Appy.Infrastructure.OnePassword.Commands
{
    public class SignInOnePasswordCommandValidator: IValidator<SignInOnePasswordCommand>
    {
        public ValidationResult Validate(SignInOnePasswordCommand command)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(command.Organization))
                return result.WithError(nameof(command.Organization).ToLower(), "1Password Organization must be specified.");

            if (!command.IsFirstSignIn)
                return result;

            if (string.IsNullOrWhiteSpace(command.Email))
                return result.WithError(nameof(command.Email).ToLower(), "1Password Email must be specified.");

            if (string.IsNullOrWhiteSpace(command.SecretKey))
                return result.WithError(nameof(command.Email).ToLower(), "1Password Secret Key must be specified.");

            return result;
        }
    }
}
