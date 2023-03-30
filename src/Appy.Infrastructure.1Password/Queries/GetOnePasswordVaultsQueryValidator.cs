using Appy.Configuration.Validation;

namespace Appy.Infrastructure.OnePassword.Queries;

public class GetOnePasswordVaultsQueryValidator: IValidator<GetOnePasswordVaultsQuery>
{
    public ValidationResult Validate(GetOnePasswordVaultsQuery query)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(query.Organization))
            return result.WithError(nameof(query.Organization).ToLower(), "1Password Organization must be specified.");

        if (string.IsNullOrWhiteSpace(query.SessionToken))
            return result.WithError(nameof(query.SessionToken).ToLower(), "1Password SessionToken must be specified.");

        return result;
    }
}