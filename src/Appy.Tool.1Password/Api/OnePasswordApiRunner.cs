using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Appy.Tool.OnePassword.Api;

public class OnePasswordApiRunner : IOnePasswordApiRunner
{
    IHost? _host;

    public static IHostBuilder CreateHostBuilder()
    {
        return new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseKestrel()
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.None);
                    })
                    .SuppressStatusMessages(true)
                    .UseEnvironment(Environments.Production)
                    .UseStartup<OnePasswordApiStartup>();
            });
    }

    public void Start(OnePasswordApiSettings settings)
    {
        if (IsRunning())
            throw new Exception("OnePassword Api already started");

        _host = CreateHostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseUrls($"http://*:{settings.Port}");
            })
            .Build();

        if (!settings.StartWithoutBlocking)
        {
            _host.Run();
            return;
        }

        _ = _host.RunAsync();
    }

    public bool IsRunning() => _host != null;

    public Task Stop()
    {
        if (!IsRunning())
            return Task.CompletedTask;

        return _host!.StopAsync();
    }
}