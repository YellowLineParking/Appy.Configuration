using System;

namespace Appy.Infrastructure.OnePassword.Tooling
{
    public class OnePasswordToolException : Exception
    {
        public OnePasswordToolException(string message)
            : base(message)
        { }

        public OnePasswordToolException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}