using System;

namespace Appy.Configuration.Validation
{
    public class ValidationException : Exception
    {
        public ValidationResult Result { get; }

        public ValidationException(string message, ValidationResult result)
            : base(message)
        {
            Result = result;
        }

        public ValidationException(ValidationResult result)
        {
            Result = result;
        }
    }
}