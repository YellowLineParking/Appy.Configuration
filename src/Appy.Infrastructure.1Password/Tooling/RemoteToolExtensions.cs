using System;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.ApiClient;
using Flurl.Http;

namespace Appy.Infrastructure.OnePassword.Tooling
{
    internal static class RemoteToolExtensions
    {
        internal static async Task<T> UnWrap<T>(this Task<Response<T>> task)
        {
            string? message;
            object? exceptionResult = null;
            Exception innerException = null!;

            try
            {
                var response = await task;

                if (response.Success)
                {
                    return response.Result;
                }

                message = response?.Message!;
            }
            catch (FlurlHttpException httpException)
            {
                var response = await httpException.GetResponseJsonAsync<Response>();
                message = response?.Message;
                exceptionResult = response;
                innerException = httpException;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                innerException = ex;
            }

            throw new OnePasswordToolException(message!, exceptionResult!, innerException);
        }
    }
}