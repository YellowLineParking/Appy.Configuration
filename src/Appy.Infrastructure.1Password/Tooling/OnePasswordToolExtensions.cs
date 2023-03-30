using Appy.Configuration.Validation;
using Appy.Infrastructure.OnePassword.Storage;

namespace Appy.Infrastructure.OnePassword.Tooling;

public static class OnePasswordToolExtensions
{
    public static OnePasswordToolConfigurationDecorator WithConfiguration(this IOnePasswordTool tool, IOnePasswordSessionStorage sessionStorage)
    {
        return new OnePasswordToolConfigurationDecorator(tool, sessionStorage);
    }

    public static OnePasswordToolValidationDecorator WithValidation(this IOnePasswordTool tool, IValidator validator)
    {
        return new OnePasswordToolValidationDecorator(tool, validator);
    }
}