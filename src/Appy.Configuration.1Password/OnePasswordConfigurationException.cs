using System;

namespace Appy.Configuration.OnePassword
{
    public class OnePasswordConfigurationException : Exception
    {
        public OnePasswordConfigurationException(string message)
            : base(message)
        { }

        public OnePasswordConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}