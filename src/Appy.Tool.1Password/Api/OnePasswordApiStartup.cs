using Appy.Tool.OnePassword.Composition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword.Api
{
    public class OnePasswordApiStartup
    {
        public void ConfigureServices(IServiceCollection services) => services
            .AddRouting()
            .AddApiDependencies();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) => app
            .UseErrorHandling(env)
            .UseRouting()
            .UseEndpoints(endpoints => endpoints.Map1PasswordEndpoints());
    }
}