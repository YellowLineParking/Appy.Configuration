using Appy.Configuration.Validation;

namespace Appy.Infrastructure.OnePassword.Queries;

public class FetchOnePasswordVaultsQueryValidator: IValidator<FetchOnePasswordVaultsQuery>
{
    public ValidationResult Validate(FetchOnePasswordVaultsQuery query)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(query.UserId))
            return result.WithError(nameof(query.UserId).ToLower(), "1Password UserId must be specified.");

        if (string.IsNullOrWhiteSpace(query.SessionToken))
            return result.WithError(nameof(query.SessionToken).ToLower(), "1Password SessionToken must be specified.");

        return result;
    }
}