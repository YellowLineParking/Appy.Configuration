using System.Collections.Generic;

namespace Appy.Infrastructure.OnePassword.ApiClient
{
    public class Response
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public IReadOnlyCollection<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Property { get; set; }
        public string Message { get; set; }
    }

    public class Response<T> : Response
    {
        public T Result { get; set; }
    }
}