using System;

namespace Appy.Infrastructure.OnePassword.ApiClient;

public class OnePasswordApiClientException : Exception
{
    public Response Response { get; }

    public int? StatusCode { get; }

    public OnePasswordApiClientException(int? statusCode, Response response)
    {
        StatusCode = statusCode;
        Response = response;
    }
}