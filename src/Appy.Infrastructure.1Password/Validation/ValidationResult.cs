using System.Collections.Generic;

namespace Appy.Infrastructure.OnePassword.Validation
{
    public class ValidationResult
    {
        public IList<ValidationError> Errors { get; set; } = new List<ValidationError>();
        public bool IsValid => Errors.Count == 0;

        public ValidationResult WithError(string property, string message)
        {
            Errors.Add(new ValidationError
            {
                Property = property,
                Message = message
            });

            return this;
        }
    }
}