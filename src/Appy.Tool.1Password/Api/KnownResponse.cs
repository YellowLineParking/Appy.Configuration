using System.Collections.Generic;
using Appy.Infrastructure.OnePassword.ApiClient;

namespace Appy.Tool.OnePassword.Api;

internal static class KnownResponse
{
    internal static Response<T> Ok<T>(T result) => new()
    {
        Success = true,
        Result = result
    };

    internal static Response BadRequest(string message, IReadOnlyCollection<Error> errors = null!) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };

    internal static Response BadRequest(IReadOnlyCollection<Error> errors) => new()
    {
        Success = false,
        Errors = errors
    };

    internal static Response Failed(string message) => new()
    {
        Success = false,
        Message = message
    };
}