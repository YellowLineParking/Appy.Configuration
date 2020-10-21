# `Appy.Configuration`

![AppyWay logo](resources/appyway-100x100.png)

## What is Appy.Configuration?

Configuration providers for NETCore 2.2, 3.0 and 3.1+.

## Configuration Providers

| Package | Latest Stable |
| --- | --- |
| [Appy.Configuration.WinRegistry](https://www.nuget.org/packages/Appy.Configuration.WinRegistry) | [![Nuget Package](https://img.shields.io/badge/nuget-0.2.0-blue.svg)](https://www.nuget.org/packages/Appy.Configuration.WinRegistry) |
| [Appy.Configuration.1Password](https://www.nuget.org/packages/Appy.Configuration.1Password) | [![Nuget Package](https://img.shields.io/badge/nuget-0.2.0-blue.svg)](https://www.nuget.org/packages/Appy.Configuration.1Password) |
| [appy-op](https://www.nuget.org/packages/appy-op) | [![Nuget Package](https://img.shields.io/badge/nuget-0.2.0-blue.svg)](https://www.nuget.org/packages/appy-op) |

## Table of Contents

- [Windows Registry Configuration Provider](#windows-registry-configuration-provider)
    * [Installing](#installing)
    * [Usage](#usage)
- [1Password Configuration Provider](#1password-configuration-provider)        
    * [Installing](#installing-1)
    * [Usage](#usage-1)   
- [Appy 1Password Tool](#appy-1password-tool)
    * [Prerequisites](#prerequisites)
    * [Installing](#installing-2)
    * [Signin to 1Password](#signin-to-1password)      

## Windows Registry Configuration Provider

The Windows registry has been with us for a long time and has served us well. Especially when we work locally or try to debug a project.

With this extension you can easily configure the loading of values from a section of your windows registry to your NETCore project configuration builder.

### Installing 

Install using the [Appy.Configuration.WinRegistry NuGet package](https://www.nuget.org/packages/Appy.Configuration.WinRegistry):

```
PM> Install-Package Appy.Configuration.WinRegistry
```

### Usage

When you install the package, it should be added to your _csproj_ file. Alternatively, you can add it directly by adding:

```xml
<PackageReference Include="Appy.Configuration.WinRegistry" Version="0.1.0" />
```

Now let's imagine we have a configuration file like the following appSettings.json:

```json
"Database": {
    "ConnectionString": ""
}
```

And a user windows registry section like:

```csharp
HKEY_CURRENT_USER\SOFTWARE\YOUR_ORG\Settings
```

With the following values:

```csharp
Database:ConnectionString: "Data Source=(LocalDb)\\mssqllocaldb;Initial Catalog=local-org-database;Integrated Security=True"
```

Then, the only thing we need to do is register an action to load the configuration values on our Program.cs file. 

```csharp
public class Program
{
    public static void Main(string[] args) =>    
        CreateWebHostBuilder(args).Build().Run();    

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, builder) => 
            {
                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    builder.AddRegistrySection(() => Microsoft.Win32.Registry.CurrentUser, "Software\\YOUR_ORG\\Settings");
                }
            }
            .UseStartup<Startup>();
}    
```

And this way we will have our configuration values ready to use like with any appsettings.json:

```csharp
public class Startup
{
    readonly IWebHostEnvironment _host;
    readonly IConfiguration _config;

    public Startup(IConfiguration config, IWebHostEnvironment host)
    {
        _host = host;
        _config = config;
    }
        
    public void ConfigureServices(IServiceCollection services)
    {
        var databaseSettings = new DatabaseSettings();
        
        var databaseSettings = _config.GetSection("Database").Bind(databaseSettings);
        ....
    }
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
}
```

To switch between environments, we would have a registry key with the settings for each one, like:

```
QA   -> "Software\\YOUR_ORG\\QA_Settings\\Database:ConnectionString"
LIVE -> "Software\\YOUR_ORG\\LIVE_Settings\\Database:ConnectionString"
```

Then we would only have to rename the registry environment folder key that we want to 'Settings', 
every time we need it and return the previous one to its original name.

```
QA   -> "Software\\YOUR_ORG\\QA_Settings" -> rename -> "Software\\YOUR_ORG\\Settings"      (We want to work with QA)
LIVE -> "Software\\YOUR_ORG\\Settings"    -> rename -> "Software\\YOUR_ORG\\LIVE_Settings" (LIVE back to his normal name)
```

Apart of all these, we could then simply create an extension to load all our configurations in just one line,
with the windows registry configuration in Development and the rest of the appSettings configurations.

```csharp
public class Program
{
    public static void Main(string[] args) =>    
        CreateWebHostBuilder(args).Build().Run();    

    public static IHostBuilder CreateWebHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .AddYourOrgAppConfiguration()
            .UseStartup<Startup>();
}    
```

Then, you can create some configuration extensions for your organization and override the config values in order. All these if necessary, 
can be preconfigured in a nuget package for your organization, which each developer can use later in their projects.

```csharp
public static class YourOrgConfigurationExtensions
{
    public static IConfigurationBuilder AddYourOrgRegistrySection(
        this IConfigurationBuilder builder,
        Action<WinRegistryConfigurationSource> configureSource = null)
    {
        return builder.AddRegistrySection(() =>
                Microsoft.Win32.Registry.CurrentUser, "Software\\YOUR_ORG\\QA_Settings");
    }

    public static IHostBuilder AddYourOrgAppConfiguration(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddYourOrgConfigurationBuilders(hostingContext.HostingEnvironment);
        });

        return hostBuilder;
    }

    public static IConfigurationBuilder AddYourOrgConfigurationBuilders(this IConfigurationBuilder builder, IHostEnvironment env)
    {
        builder
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        if (env.IsDevelopment())
        {
            builder
                .AddYourOrgRegistrySection();
        }

        return builder;
    }
}
```

You can find more examples on the samples folder.

## 1Password Configuration Provider

1Password is one of those services that makes our life easier every day, storing all our passwords in a secure manner.

With this extension you can extend the power of 1Password and easily configure the loading of settings from 1Password to your NETCore project configuration builder.

### Installing 

Install using the [Appy.Configuration.1Password NuGet package](https://www.nuget.org/packages/Appy.Configuration.1Password):

```
PM> Install-Package Appy.Configuration.1Password
```

### Usage

When you install the package, it should be added to your _csproj_ file. Alternatively, you can add it directly by adding:

```xml
<PackageReference Include="Appy.Configuration.1Password" Version="0.1.0" />
```

Let's imagine we have a configuration file like the following appsettings.json file:

```json
"Database": {
    "ConnectionString": ""
}
```

And your project settings on 1Password. You should create a secure note on your personal or organization vault with a section and settings for each environment you need.

![Secure Note Example](resources/screenshots/op-note-appsettings.png)

With the following values:

```csharp
Database:ConnectionString: "Data Source=(LocalDb)\\mssqllocaldb;Initial Catalog=local-org-database;Integrated Security=True"
```

Then, the only thing we need to do is register an action to load the configuration values on our Program.cs. 

```csharp
public class Program
{
    public static void Main(string[] args) =>    
        CreateWebHostBuilder(args).Build().Run();    

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, builder) => 
            {                
                builder
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    // Load 1Password settings following the Appy conventions with Appy 1Password Tool.
                    builder.Add1Password("Appy.Sample.1Password.Api");
                }
            }
            .UseStartup<Startup>();
}    
```

And this way we will have our configuration values ready to use like with any appsettings.json:

```csharp
public class Startup
{
    readonly IWebHostEnvironment _host;
    readonly IConfiguration _config;

    public Startup(IConfiguration config, IWebHostEnvironment host)
    {
        _host = host;
        _config = config;
    }
        
    public void ConfigureServices(IServiceCollection services)
    {
        var databaseSettings = new DatabaseSettings();
        
        var databaseSettings = _config.GetSection("Database").Bind(databaseSettings);
        ....
    }
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
}
```

Then would come the part where we communicate with 1Password and get these settings. We have created a command line tool that facilitates the process in a 
single command and allows you to create a session with some pre-configured settings. More on that below [Appy 1Password Tool](#appy-1password-tool).

You can find more examples on the samples folder.

## Appy 1Password Tool

The Appy 1Password Tool is a dotnet tool that works as a wrapper around the official [1Password command-line tool](https://1password.com/downloads/command-line/). Following some basic conventions it will help you to start a 1Password session, so later you can run and debug locally any dotnet project using the preconfigured AppSettings saved on 1Password.

The tool allows you to create a session and later set the following user environment variables to be loaded by your project through the 1Password Configuration Provider extensions:

User environment variables conventions:

```
- appy_op_organization
- appy_op_vault
- appy_op_env
- appy_op_session_token
```

### Prerequisites

First, we should install the official [1Password CLI](https://support.1password.com/command-line/). 

#### Windows

For an easy installation, we recommend that you first install [Chocolatey Package Manager](https://chocolatey.org/install).

Open a Powershell console and install 1Password CLI:

```console
choco install op
```

#### MacOS

For an easy installation, we recommend that you first install [Brew Package Manager](https://brew.sh/).

Open a console and install 1Password CLI using brew cask:

```console
brew cask install 1password-cli
```

#### Linux

Please, follow the official ['Getting started docs from 1Password'](https://support.1password.com/command-line-getting-started/)

## Installing

Install the tool globally.

```console
dotnet tool install -g appy-op
```

## Signin to 1Password

### First Time access

Execute the next command to signin to 1Password and set the vault and environment to load:

```console
appy-op --signin <yourorg> <email-address> <secret_key> --vault Development -env QA
```
*Note: make sure to use the Secret Key and not the Master Password*

You can get these data from your 1Password Desktop or Mobile App account details.

Then, it will ask for your Master Password.

A normal session will look like this:

```console
appy-op --signin yourorg your_name@yourorg.com secretkey --vault Development -env QA

Enter the password for your_name@yourorg.com at yourorg.1password.com:
...

Appy 1Password session started:
+------------------------------------------------------------+
| Organization | youorg                                      |
| Vault        | Development                                 |
| Environment  | DEV                                         |
| SessionToken | 1password session token                     |
+------------------------------------------------------------+

You can now go to your project and start your debug session.
Session tokens expire after 30 minutes of inactivity, after which you'll need to sign in again.
```

### Later time access

For subsequent accesses, it will only be necessary the next command:

```console
appy-op --signin (or -s)
```
or if you want to change the current vault or environment:

```console
appy-op -s -vt Custom -env DEV
```

## Contribute
It would be awesome if you would like to contribute code or help with bugs. Just follow the guidelines [CONTRIBUTING](https://github.com/YellowLineParking/Appy.Configuration/blob/master/CONTRIBUTING.md).

## Additional Resources
* [WinRegistry Configuration based on GeorgeTsaplin project](https://github.com/GeorgeTsaplin/Configuration.WinRegistry)