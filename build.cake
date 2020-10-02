#addin nuget:?package=Cake.Yaml&version=3.1.1
#addin nuget:?package=YamlDotNet&version=8.1.2

#load "./build/functions.cake"

var configFilePath = "config.yml";
var taskConfigManager = new ProjectTaskConfigurationManager();
var projectConfigs = new ProjectConfigLoader().Load(Context, configFilePath).Projects;
var basePath = "./src";
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

////////////////////////////////////////////////////////////////
// Setup

Setup((context) =>
{
    Information("AppyWay");
});

////////////////////////////////////////////////////////////////
// Tasks

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore(basePath,
        new DotNetCoreRestoreSettings
        {
            Verbosity = DotNetCoreVerbosity.Minimal
        });
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(context =>
{
    foreach(var projectConfig in projectConfigs)
    {
        if (!taskConfigManager.CanBuild(projectConfig)) continue;

        DotNetCoreBuild($"{basePath}/{projectConfig.Name}/{projectConfig.Name}.csproj", new DotNetCoreBuildSettings {
            Configuration = configuration,
            NoRestore = true,
            NoIncremental = context.HasArgument("rebuild"),
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
        });
    }
});

Task("Test")
    .IsDependentOn("Build")
    .Does(context =>
{
    // DotNetCoreTest("./src/Appy.Configuration.Tests/Appy.Configuration.Tests.csproj", new DotNetCoreTestSettings {
    //     Configuration = configuration,
    //     NoRestore = true,
    //     NoBuild = true,
    // });
});

Task("Package")
    .IsDependentOn("Test")
    .Does(context =>
{
    context.CleanDirectory("./.artifacts");

    foreach(var projectConfig in projectConfigs)
    {
        if (!taskConfigManager.CanPack(projectConfig)) continue;

        context.DotNetCorePack($"{basePath}/{projectConfig.Name}/{projectConfig.Name}.csproj", new DotNetCorePackSettings {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            OutputDirectory = "./.artifacts",
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
        });
    }
});

Task("Publish-GitHub")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context =>
{
    var apiKey = Argument<string>("github-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No GitHub API key was provided.");
    }

    // Publish to GitHub Packages
    var exitCode = 0;
    foreach(var file in context.GetFiles("./.artifacts/*.nupkg"))
    {
        context.Information("Publishing {0}...", file.GetFilename().FullPath);
        exitCode += StartProcess("dotnet",
            new ProcessSettings {
                Arguments = new ProcessArgumentBuilder()
                    .Append("gpr")
                    .Append("push")
                    .AppendQuoted(file.FullPath)
                    .AppendSwitchSecret("-k", " ", apiKey)
            }
        );
    }

    if(exitCode != 0)
    {
        throw new CakeException("Could not push GitHub packages.");
    }
});

Task("Publish-NuGet")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context =>
{
    var apiKey = Argument<string>("nuget-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No NuGet API key was provided.");
    }

    // Publish to GitHub Packages
    foreach(var file in context.GetFiles("./.artifacts/*.nupkg"))
    {
        context.Information("Publishing {0}...", file.GetFilename().FullPath);
        DotNetCoreNuGetPush(file.FullPath, new DotNetCoreNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = apiKey,
        });
    }
});

////////////////////////////////////////////////////////////////
// Targets

Task("Publish")
    .IsDependentOn("Publish-GitHub")
    .IsDependentOn("Publish-NuGet");

Task("Default")
    .IsDependentOn("Package");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target)

