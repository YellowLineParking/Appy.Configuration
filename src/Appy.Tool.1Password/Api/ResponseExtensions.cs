using System.Collections.Generic;
using System.Linq;
using Appy.Configuration.Validation;
using Appy.Infrastructure.OnePassword.ApiClient;
using static Appy.Tool.OnePassword.Api.KnownResponse;

namespace Appy.Tool.OnePassword.Api;

internal static class ResponseExtensions
{
    internal static Response<T> ToResponse<T>(this T result) => Ok(result);
    internal static Response ToBadResponse(this IReadOnlyCollection<Error> errors) => BadRequest("The request is invalid", errors);
    internal static Response ToErrorResponse(this string message) => Failed(message);
    internal static Response ToBadResponse(this ValidationResult result)
    {
        var errors = result.Errors
            .Select(ve => new Error
            {
                Property = ve.Property,
                Message = ve.Message
            })
            .ToList();

        return errors.ToBadResponse();
    }
}