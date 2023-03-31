using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tooling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword.Api;

public static class OnePasswordToolEndpoints
{
    public static void Map1PasswordEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var serializer = endpoints.ServiceProvider.GetService<IAppyJsonSerializer>();
        var tool = endpoints.ServiceProvider.GetService<IOnePasswordTool>();

        endpoints.MapPost("/queries/fetchOnePasswordNote", async context =>
        {
            var query = await context.Request.ReadJsonAsync<FetchOnePasswordNoteQuery>(serializer);
            var result = await tool.Execute(query);
            await context.Response.WriteOkAsync(serializer, result.ToResponse());
        });
    }
}