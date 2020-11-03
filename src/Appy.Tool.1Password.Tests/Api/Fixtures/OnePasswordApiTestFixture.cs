using System;
using Appy.Tool.OnePassword.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Appy.Tool.OnePassword.Tests.Api.Fixtures
{
    public class OnePasswordApiTestFixture : WebApplicationFactory<OnePasswordApiStartup>
    {
        ITestOutputHelper _output;

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return OnePasswordApiRunner
                .CreateHostBuilder()
                .UseEnvironment(Environments.Development)
                .ConfigureLogging(logging => logging
                    .ClearProviders()
                    .AddXUnit(_output));
        }

        public OnePasswordApiTestFixture WithOutput(ITestOutputHelper output)
        {
            _output = output;
            return this;
        }

        public Action<IServiceCollection> ServicesConfiguration { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                ServicesConfiguration?.Invoke(services);
            });
        }

        protected override void Dispose(bool disposing)
        {
            _output = null;
            base.Dispose(disposing);
        }
    }
}