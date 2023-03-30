using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Appy.Configuration.WinRegistry.Tests;

#pragma warning disable CA1416

public class WinRegistryConfigurationTests
{
    [Fact(Skip = "Manual execution only")]
    public void ShouldReadSingleValueFromRegistrySection()
    {
        var target = new WinRegistryConfigurationProvider(new WinRegistryConfigurationSource(
            () => Microsoft.Win32.Registry.LocalMachine,
            "SOFTWARE\\Microsoft\\Shell"));

        target.Load();

        string actual;

        target.TryGet("USB:NotifyOnUsbErrors", out actual);

        actual.Should().Be("1");
    }

    [Fact(Skip = "Manual execution only")]
    public void ShouldReadNestedObjectFromRegistrySection()
    {
        var configBuilder = new ConfigurationBuilder();
        var config = configBuilder.AddRegistrySection(() =>
                Microsoft.Win32.Registry.CurrentUser, "Software\\YOUR_ORG\\Settings")
            .Build();

        var expected = new DatabaseSettings
        {
            ConnectionString = "Data Source=(LocalDb)\\mssqllocaldb;Initial Catalog=local-org-database;Integrated Security=True"
        };

        var databaseSettings = new DatabaseSettings();

        config.GetSection("Database").Bind(databaseSettings);

        databaseSettings.Should().BeEquivalentTo(expected);
    }

#pragma warning restore CA1416

    private class DatabaseSettings
    {
        public string ConnectionString { get; set; }
    }
}