using System;

namespace Appy.Configuration.Validation;

public class DefaultValidator: IValidator
{
    readonly IValidationProvider _validationProvider;

    public DefaultValidator(IValidationProvider validationProvider)
    {
        _validationProvider = validationProvider ?? throw new ArgumentNullException(nameof(validationProvider));
    }

    public ValidationResult Validate<TModel>(TModel model)
    {
        if (ReferenceEquals(model, null))
            throw new ArgumentNullException(nameof(model));

        var validator = _validationProvider.GetValidator<TModel>();

        return validator.Validate(model);
    }
}