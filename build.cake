#addin nuget:?package=YamlDotNet&version=16.2.0
#addin nuget:?package=System.Xml.XDocument&version=4.3.0
#addin nuget:?package=Cake.MinVer&version=4.0.0
#addin nuget:?package=Cake.Yaml&version=6.0.0
#addin nuget:?package=Cake.Docker&version=1.3.0

#load "./functions.cake"

var basePath = "./src";
var organization = "appyway";
var artifactsPath = Context.Directory("./.artifacts");
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var configFilePath = "config.yml";
var transversalBuildFilePath = $"{basePath}/build.csproj";
var taskConfigManager = new ProjectTaskConfigurationManager();
var projectDescriptors = ProjectLoader.Load(Context, configFilePath, basePath, configuration).Projects;
var version = MinVer(settings => settings
    .WithDefaultPreReleasePhase("preview")
    .WithVerbosity(MinVerVerbosity.Info));

////////////////////////////////////////////////////////////////
// Setup

Setup((context) =>
{
    Information("AppyWay");
    Information($"Version: {version.Version}");
});

////////////////////////////////////////////////////////////////
// Tasks

Task("Clean")
    .Does(context =>
{
    context.CleanDirectory(artifactsPath);
});

Task("Restore")
    .Does(() =>
{
    if (FileExists(transversalBuildFilePath))
    {
        return;
    }

    DotNetRestore(basePath,
        new DotNetRestoreSettings
        {
            Verbosity = DotNetVerbosity.Minimal
        });
});

Task("Build-Project")
    .IsDependentOn("Restore")
    .Does(context =>
{
    if (FileExists(transversalBuildFilePath))
    {
        context.Information("Building projects using Transversal Build");

        DotNetBuild(transversalBuildFilePath, new DotNetBuildSettings {
            Configuration = configuration,
            // NoRestore = true,
            // NoIncremental = context.HasArgument("rebuild"),
            Verbosity = DotNetVerbosity.Minimal,
            MSBuildSettings = new DotNetMSBuildSettings()
                .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
        });

        return;
    }

    foreach(var projectDescriptor in projectDescriptors)
    {
        if (!taskConfigManager.CanBuild(projectDescriptor.Config)) continue;

        var projectFilePath = projectDescriptor.Document.ProjectFileFullPath;

        DotNetBuild(projectFilePath, new DotNetBuildSettings {
            Configuration = configuration,
            NoRestore = true,
            NoIncremental = context.HasArgument("rebuild"),
            MSBuildSettings = new DotNetMSBuildSettings()
                .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
        });
    }
});

Task("Build-Docker")
    .Does(context =>
{
    foreach(var projectDescriptor in projectDescriptors)
    {
        if (!taskConfigManager.CanBuildDocker(projectDescriptor.Config)) continue;

        var dockerFilePath = context.BuildDockerFilePath(basePath, projectDescriptor.Config);

        if (!FileExists(dockerFilePath)) continue;

        var repository = $"{organization}/{projectDescriptor.Document.DotNet.PackageId}";

        var tags = new[] { version.Version, "latest" };

        var settings = new DockerImageBuildSettings
        {
            File = dockerFilePath,
            BuildArg = new[] { $"configuration={configuration}" },
            Pull = false,
            Tag = tags.Select(tag => $"{repository}:{tag}").ToArray()
        };

        DockerBuild(settings, ".");
    }
});

Task("Test")
    .IsDependentOn("Build-Project")
    .Does(context =>
{
    if (FileExists(transversalBuildFilePath))
    {
        context.Information("Testing projects using Transversal Build");

       DotNetTest(transversalBuildFilePath, new DotNetTestSettings {
           Configuration = configuration,
           NoRestore = true,
           NoBuild = true,
           TestAdapterPath = ".",
           Loggers = new string[] {
             "GitHubActions;report-warnings=false"
           },
           Verbosity = DotNetVerbosity.Quiet
        });

       return;
    }

    foreach(var projectDescriptor in projectDescriptors)
    {
        if (!taskConfigManager.CanTest(projectDescriptor.Config)) continue;

        var projectFilePath = projectDescriptor.Document.ProjectFileFullPath;

        DotNetTest(projectFilePath, new DotNetTestSettings {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            TestAdapterPath = ".",
            Loggers = new string[] {
                // $"xunit;LogFilePath={MakeAbsolute(artifactsPath).FullPath}/xunit-{projectDescriptor.Config.Name}.xml",
                "GitHubActions;report-warnings=false"
            },
            Verbosity = DotNetVerbosity.Quiet
        });
    }
});

Task("Package")
    .IsDependentOn("Test")
    .Does(context =>
{

    if (FileExists(transversalBuildFilePath))
    {
        context.Information("Packing projects using Transversal Build");

        context.DotNetPack(transversalBuildFilePath, new DotNetPackSettings {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            OutputDirectory = artifactsPath,
            MSBuildSettings = new DotNetMSBuildSettings()
              .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
        });

       return;
    }

    foreach(var projectDescriptor in projectDescriptors)
    {
        if (!taskConfigManager.CanPack(projectDescriptor.Config)) continue;

        var projectFilePath = projectDescriptor.Document.ProjectFileFullPath;

        context.DotNetPack(projectFilePath, new DotNetPackSettings {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            OutputDirectory = artifactsPath,
            MSBuildSettings = new DotNetMSBuildSettings()
                .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
        });
    }
});

Task("Publish-Package-GitHub")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context =>
{
    var apiKey = Argument<string>("github-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No GitHub API key was provided.");
    }

    var exitCode = 0;
    foreach(var projectDescriptor in projectDescriptors)
    {
        if (!taskConfigManager.CanPack(projectDescriptor.Config)) continue;

        var nugetPkgFilePath = context.BuildNugetPackagePath(artifactsPath, projectDescriptor, version.Version);

        context.Information("Publishing {0} to Github", nugetPkgFilePath);
        exitCode += StartProcess("dotnet",
            new ProcessSettings {
                Arguments = new ProcessArgumentBuilder()
                    .Append("gpr")
                    .Append("push")
                    .AppendQuoted(nugetPkgFilePath)
                    .AppendSwitchSecret("-k", " ", apiKey)
            }
        );
    }

    if(exitCode != 0)
    {
        throw new CakeException("Could not push GitHub packages.");
    }
});

Task("Publish-Package-NuGet")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context =>
{
    var apiKey = Argument<string>("nuget-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No NuGet API key was provided.");
    }

    foreach(var projectDescriptor in projectDescriptors)
    {
        if (!taskConfigManager.CanPack(projectDescriptor.Config)) continue;

        var nugetPkgFilePath = context.BuildNugetPackagePath(artifactsPath, projectDescriptor, version.Version);

        context.Information("Publishing {0} to Nuget", nugetPkgFilePath);

        DotNetNuGetPush(nugetPkgFilePath, new DotNetNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = apiKey
        });
    }
});

Task("Publish-Docker-DockerHub")
    .IsDependentOn("Build-Docker")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    // .WithCriteria(() => Build.Version.IsPublic, "Not public")
    .Does(context =>
{
    foreach(var projectDescriptor in projectDescriptors)
    {
        if (!taskConfigManager.CanBuildDocker(projectDescriptor.Config)) continue;

        var dockerFilePath = context.BuildDockerFilePath(basePath, projectDescriptor.Config);

        if (!FileExists(dockerFilePath)) continue;

        var repository = $"{organization}/{projectDescriptor.Document.DotNet.PackageId}";

        var tags = new List<string> { version.Version };

        if (!version.IsPreRelease) {
            tags.Add("latest");
        }

        foreach(var tag in tags)
        {
            DockerPush($"{repository}:{tag}");
            // DockerTag();
        }
    }
});

////////////////////////////////////////////////////////////////
// Targets

Task("Publish")
    .IsDependentOn("Publish-Package-GitHub")
    .IsDependentOn("Publish-Package-NuGet")
    .IsDependentOn("Publish-Docker-DockerHub");

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build-Project")
    .IsDependentOn("Package");

Task("Docker")
    .IsDependentOn("Clean")
    .IsDependentOn("Build-Project")
    .IsDependentOn("Build-Docker")
    .IsDependentOn("Package");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target)