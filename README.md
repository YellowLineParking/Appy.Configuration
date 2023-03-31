# `Appy.Configuration`

![AppyWay logo](resources/appyway-100x100.png)

## What is Appy.Configuration?

Configuration providers for Dotnet.

The latest supported version of 1Password CLI is 1.12.5.

## Configuration Providers

| Package | Latest Stable |
| --- | --- |
| [Appy.Configuration.1Password](https://www.nuget.org/packages/Appy.Configuration.1Password) | [![Nuget Package](https://img.shields.io/badge/nuget-0.11.0-blue.svg)](https://www.nuget.org/packages/Appy.Configuration.1Password) |
| [appy-op](https://www.nuget.org/packages/appy-op) | [![Nuget Package](https://img.shields.io/badge/nuget-0.11.0-blue.svg)](https://www.nuget.org/packages/appy-op) 
| [appy-op (Docker Image)](https://hub.docker.com/r/appyway/appy-op/tags?page=1&ordering=last_updated) | [![Docker Image](https://img.shields.io/badge/docker-0.11.0-blue.svg)](https://hub.docker.com/r/appyway/appy-op/tags?page=1&ordering=last_updated) |
| [Appy.Configuration.WinRegistry](https://www.nuget.org/packages/Appy.Configuration.WinRegistry) | [![Nuget Package](https://img.shields.io/badge/nuget-0.11.0-blue.svg)](https://www.nuget.org/packages/Appy.Configuration.WinRegistry) |

## Table of Contents

- [1Password Configuration Provider](#1password-configuration-provider)
    * [Installing](#installing)
    * [Usage](#usage)
- [Appy 1Password Tool](#appy-1password-tool)
    * [Prerequisites](#prerequisites)
    * [Installing](#installing-1)
    * [Signin to 1Password](#signin-to-1password)
    * [Auto-renew session activity](#auto-renew-session-activity)
    * [Nano Api and Docker](#nano-api-and-docker)
    * [Tool as Docker Image](#tool-as-docker-image)
- [Windows Registry Configuration Provider](#windows-registry-configuration-provider)
    * [Installing](#installing-2)
    * [Usage](#usage-1)

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
<PackageReference Include="Appy.Configuration.1Password" Version="0.11.0" />
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

Then would come the part where we communicate with 1Password and get these settings. We have created a command line tool that facilitates the process in a single command and allows you to create a session with some pre-configured settings. More on that below [Appy 1Password Tool](#appy-1password-tool).

You can find more examples on the samples folder.

## Appy 1Password Tool

The Appy 1Password Tool is a dotnet tool that works as a wrapper around the official [1Password command-line tool](https://1password.com/downloads/command-line/). Following some basic conventions it will help you to start a 1Password session, so later you can run and debug locally any dotnet project using the preconfigured AppSettings saved on 1Password.

The tool allows you to create a session and later save the following information to be loaded by your project through the 1Password Configuration Provider extensions.

#### Windows

The session information is stored in the next user's environment variables:

```
- APPY_OP_ORGANIZATION
- APPY_OP_VAULT
- APPY_OP_ENVIRONMENT
- APPY_OP_SESSION_TOKEN
```

* No password is stored on your machine, only the data of your session in your user environment variables.

#### MacOs & Linux

Storing environment variables globally on MacOS or Linux always seems complicated, and management varies a lot from one console to another. For this reason, the session information is store in a file (.appy-op-env) in the root folder of the current user.

The file will contain the following information:

```
APPY_OP_ORGANIZATION=yourorg
APPY_OP_VAULT=Development
APPY_OP_ENVIRONMENT=DEV
APPY_OP_SESSION_TOKEN=1password session token
```

* No password is stored on your machine, only the data of your session.

### Prerequisites

First, we should install the official [1Password CLI](https://support.1password.com/command-line/).

#### Windows

For an easy installation, we recommend that you first install [Chocolatey Package Manager](https://chocolatey.org/install).

Open a Powershell console and install 1Password CLI:

```console
choco install op --version=1.12.5 
```

#### MacOS

For an easy installation, we recommend that you first install [Brew Package Manager](https://brew.sh/).

Open a console and install 1Password CLI using brew cask:

```console
brew install 1password-cli1
```

#### Linux

Please, follow the official ['Getting started docs from 1Password'](https://support.1password.com/command-line-getting-started/)

### Installing

Install the tool globally.

```console
dotnet tool install -g appy-op
```

### Signin to 1Password

#### First Time access

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

Updating 1Password session information.

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

#### Later time access

For subsequent accesses, it will only be necessary the next command:

```console
appy-op --signin (or -s)
```
or if you want to change the current vault or environment:

```console
appy-op -s -vt Custom -env DEV
```

### Auto-renew session activity

1Password sessions have an expiration time of 30 min if there is no activity.
Normally that will be enough, since during our project debug session, every time we
load the project settings the session will be renewed for another 30 min.

But for cases where we are a bit lazy, or want to go outside for a long time and want to leave
our session active, we have the option to signin with auto-renew. That will try to keep the session
active by making a simple random query to 1Password every 29 minutes.

```console
appy-op -s -a
```

### Nano Api and Docker

In search of supporting projects that want to run or debug their projects inside Docker and at the same time consume the configuration from 1Password, a simple Nano Api integrated directly in the tool was added. This eliminates the need to have any reference to the 1Password cli in your project image.

```console
appy-op -s -a -api 5500
```

When executing or debugging your dotnet project in a Docker container, it will be necessary to specify the url of the tool's Api through an environment variable:

```
APPY_OP_API_URI
```

If this variable is set, the configuration requests are automatically redirected to the API instead of calling the 1Password cli directly from the project.

An example of execution could be the following:

```console
docker run \
--env ASPNETCORE_ENVIRONMENT=Development \
--env APPY_OP_API_URI=http://host.docker.internal:5500/ \
--name appysample1passwordapi \
dev
```

Where the url points to the special DNS name host.docker.internal which resolves to the internal IP address used by the host, where the tool will be running.

This option create an opportunity, where we can consume this api from projects in different programming languages.

### Tool as Docker Image

In case you want to use the appy-op as a docker image and thus avoid installing the 1Password cli or any dotnet dependency on your system, you can use the next command to run the image.

```console
docker run --rm -it -p 6000:6000 --name appy-op-test \
appyway/appy-op -s <yourorg> <email-address> <secret_key> -vt Development -env DEV -a -api 6000
```

And to use appy-op with your host machine's existing config (or to persist configuration after the container exits):

First time access:

```console
docker run --rm -it -e "HOME=$HOME" -v "$HOME:$HOME" -p 6000:6000 --name appy-op-test \
appyway/appy-op -s <yourorg> <email-address> <secret_key> -vt Development -env DEV -a -api 6000
```

Later time access:

```console
docker run --rm -it -e "HOME=$HOME" -v "$HOME:$HOME" -p 6000:6000 --name appy-op-test \
appyway/appy-op -s -vt Development -env DEV -a -api 6000
```

Or you could create your own script and add it to the bin folder to simplify the process.

To communicate from your project with the tool, you could simply call the api as explained in the previous
section, or create a shared network between your projects and the tool.

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
<PackageReference Include="Appy.Configuration.WinRegistry" Version="0.11.0" />
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

## Contribute
It would be awesome if you would like to contribute code or help with bugs. Just follow the guidelines [CONTRIBUTING](https://github.com/YellowLineParking/Appy.Configuration/blob/master/CONTRIBUTING.md).

## Additional Resources
* [WinRegistry Configuration based on GeorgeTsaplin project](https://github.com/GeorgeTsaplin/Configuration.WinRegistry)