using Appy.Configuration.Validation;

namespace Appy.Infrastructure.OnePassword.Queries;

public class FetchOnePasswordNoteQueryValidator : IValidator<FetchOnePasswordNoteQuery>
{
    public ValidationResult Validate(FetchOnePasswordNoteQuery query)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(query.Item))
            return result.WithError(nameof(query.Item).ToLower(), "1Password AppName must be specified.");

        if (string.IsNullOrWhiteSpace(query.UserId))
            return result.WithError(nameof(query.UserId).ToLower(), "1Password UserId must be specified.");

        if (string.IsNullOrWhiteSpace(query.Vault))
            return result.WithError(nameof(query.Vault).ToLower(), "1Password Vault must be specified.");

        if (string.IsNullOrWhiteSpace(query.Environment))
            return result.WithError(nameof(query.Environment).ToLower(), "1Password Environment must be specified.");

        if (string.IsNullOrWhiteSpace(query.SessionToken))
            return result.WithError(nameof(query.SessionToken).ToLower(), "1Password SessionToken must be specified.");

        return result;
    }
}