using System;
using System.Threading.Tasks;
using Appy.Configuration.Serializers;
using Appy.Configuration.Validation;
using Appy.Infrastructure.OnePassword.Tooling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword.Api;

internal static class ErrorHandlingMiddleware
{
    internal static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                await ApiErrorHandler.HandleExceptionAsync(app, context, ex, env);
            }
        });

        return app;
    }

    static class ApiErrorHandler
    {
        internal static Task HandleExceptionAsync(IApplicationBuilder app, HttpContext context, Exception exception, IWebHostEnvironment env)
        {
            var serializer = app.ApplicationServices.GetService<IAppyJsonSerializer>();

            if (exception is ValidationException validationException)
            {
                return context.Response.WriteBadRequestAsync(serializer, validationException.Result.ToBadResponse());
            }

            if (exception is OnePasswordToolException toolException)
            {
                return context.Response.WriteErrorAsync(serializer, toolException.Message.ToErrorResponse());
            }

            return context.Response.WriteErrorAsync(serializer, "Internal server error".ToErrorResponse());
        }
    }
}