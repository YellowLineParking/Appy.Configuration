using System.IO;
using System.Net;
using System.Threading.Tasks;
using Appy.Configuration.Serializers;
using Microsoft.AspNetCore.Http;

namespace Appy.Tool.OnePassword.Api
{
    internal static class HttpResponseExtensions
    {
        public static Task WriteOkAsync(this HttpResponse response, IAppyJsonSerializer serializer, object value) =>
            WriteJsonAsync(response, serializer, HttpStatusCode.OK, value);

        public static Task WriteBadRequestAsync(this HttpResponse response, IAppyJsonSerializer serializer, object value) =>
            WriteJsonAsync(response, serializer, HttpStatusCode.BadRequest, value);

        public static Task WriteErrorAsync(this HttpResponse response, IAppyJsonSerializer serializer, object value) =>
            WriteJsonAsync(response, serializer, HttpStatusCode.InternalServerError, value);

        public static async Task<T> ReadJsonAsync<T>(this HttpRequest request, IAppyJsonSerializer serializer)
        {
            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync();
            return serializer.Deserialize<T>(body);
        }

        public static async Task WriteJsonAsync(this HttpResponse response, IAppyJsonSerializer serializer, HttpStatusCode statusCode, object value, string contentType = null!)
        {
            var json = serializer.Serialize(value);
            await response.WriteJsonAsync(statusCode, json, contentType);
            await response.Body.FlushAsync();
        }

        public static async Task WriteJsonAsync(this HttpResponse response, HttpStatusCode statusCode, string json, string contentType = null!)
        {
            response.StatusCode = (int)statusCode;
            response.ContentType = contentType ?? "application/json; charset=UTF-8";
            await response.WriteAsync(json);
            await response.Body.FlushAsync();
        }
    }
}