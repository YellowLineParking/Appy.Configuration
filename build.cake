#addin nuget:?package=Cake.Yaml&version=3.1.1
#addin nuget:?package=YamlDotNet&version=8.1.2

#load "./functions.cake"

var configFilePath = "config.yml";
var taskConfigManager = new ProjectTaskConfigurationManager();
var projectConfigs = new ProjectConfigLoader().Load(Context, configFilePath).Projects;
var basePath = "./src";
var artifactsPath = Context.Directory("./.artifacts");
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

        var projectPath = BuildProjectPath(basePath, projectConfig);
        DotNetCoreBuild(projectPath, new DotNetCoreBuildSettings {
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
    foreach(var projectConfig in projectConfigs)
    {
        if (!taskConfigManager.CanTest(projectConfig)) continue;

        var projectPath = BuildProjectPath(basePath, projectConfig);
        DotNetCoreTest(projectPath, new DotNetCoreTestSettings {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            TestAdapterPath = ".",
            Logger = $"xunit;LogFilePath={MakeAbsolute(artifactsPath).FullPath}/xunit-{projectConfig.Name}.xml",
            Verbosity = DotNetCoreVerbosity.Quiet
        });
    }

});

Task("Package")
    .IsDependentOn("Test")
    .Does(context =>
{
    context.CleanDirectory("./.artifacts");

    foreach(var projectConfig in projectConfigs)
    {
        if (!taskConfigManager.CanPack(projectConfig)) continue;

        var projectPath = BuildProjectPath(basePath, projectConfig);
        context.DotNetCorePack(projectPath, new DotNetCorePackSettings {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            OutputDirectory = artifactsPath,
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

