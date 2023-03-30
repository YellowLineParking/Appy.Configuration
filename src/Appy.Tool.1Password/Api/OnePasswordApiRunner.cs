using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Appy.Tool.OnePassword.Api;

public class OnePasswordApiRunner : IOnePasswordApiRunner
{
    IWebHost _webHost;

    public static IWebHostBuilder CreateHostBuilder()
    {
        return new WebHostBuilder()
            .UseKestrel()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Warning);
                logging.SetMinimumLevel(LogLevel.None);
            })
            .SuppressStatusMessages(true)
            .UseEnvironment(Environments.Production)
            .UseStartup<OnePasswordApiStartup>();
    }

    public void Start(OnePasswordApiSettings settings)
    {
        if (IsRunning())
        {
            throw new Exception("OnePassword Api already started");
        }

        _webHost = CreateHostBuilder()
            .UseUrls($"http://*:{settings.Port}")
            .Build();

        if (!settings.StartWithoutBlocking)
        {
            _webHost.Run();
            return;
        }

        var _ = _webHost.RunAsync();
    }

    public bool IsRunning()
    {
        return _webHost != null;
    }

    public Task Stop()
    {
        if (!IsRunning())
        {
            return Task.CompletedTask;
        }

        return _webHost.StopAsync();
    }
}