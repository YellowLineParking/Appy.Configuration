using System;

namespace Appy.Infrastructure.OnePassword.Tooling;

public class OnePasswordToolException : Exception
{
    public object Result { get; }

    public OnePasswordToolException(string message)
        : base(message)
    { }

    public OnePasswordToolException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public OnePasswordToolException(string message, object result, Exception innerException)
        : base(message, innerException)
    {
        Result = result;
    }
}