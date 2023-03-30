using System.Collections.Generic;

namespace Appy.Configuration.Validation;

public class ValidationResult
{
    public IList<ValidationError>? Errors { get; private set; }
    public bool IsValid => Errors == null || Errors.Count == 0;
    public ValidationResult WithError(string property, string message)
    {
        Errors ??= new List<ValidationError>();
        Errors.Add(new ValidationError
        {
            Property = property,
            Message = message
        });

        return this;
    }
}