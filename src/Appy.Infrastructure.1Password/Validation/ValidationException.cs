using System;

namespace Appy.Infrastructure.OnePassword.Validation
{
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        { }
    }
}