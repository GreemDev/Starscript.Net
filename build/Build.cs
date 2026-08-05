using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;

partial class Build : NukeBuild
{
    public const string BaseVersion = "1.0";

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] readonly Solution Solution;
    private AbsolutePath ArtifactsPath => RootDirectory / "artifacts";

    private AbsolutePath NuGetPackageOutput => ArtifactsPath.GlobFiles("*.nupkg").FirstOrDefault();

    [Parameter("nuget.org API key", Name = "NugetApiKey")] readonly string NugetApiKey;

    public string NextVersion
    {
        get
        {
            if (field != null)
                return field;
            
            if (Host is GitHubActions gha)
            {
                // +58 accounts for the final version of pre-GRUKE CI
                // -2 accounts for the 2 failed CI runs after GRUKE migration
                return field = $"{BaseVersion}.{(gha.RunNumber + 58) - 2}";
            }

            return field = "999.0.0";
        }
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            ArtifactsPath.CreateOrCleanDirectory();

            DotNetTasks.DotNetClean(_ => _
                .SetProject(Solution.Starscript_Net)
            );

            DotNetTasks.DotNetClean(_ => _
                .SetProject(Solution.Starscript_Net_Benchmarks)
            );

            DotNetTasks.DotNetClean(_ => _
                .SetProject(Solution.Starscript_Net_TestProgram)
            );
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(_ => _
                .SetProjectFile(Solution)
            );
        });

    Target Benchmarks => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRun(_ => _
                .SetProjectFile(Solution.Starscript_Net_Benchmarks)
                .SetApplicationArguments("--ci")
                .SetConfiguration(Configuration.Release));
            
            if (Host is not GitHubActions gha) return;
            
            var fileCandidates = (RootDirectory / "benchout" / "results")
                .GlobFiles("*-github.md");

            if (fileCandidates.FirstOrDefault() is { } markdownPath)
            {
                gha.StepSummaryFile.AppendAllLines(
                    File.ReadAllLines(markdownPath)
                );
            }
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(_ => _
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsPath)
                .SetVersion(NextVersion)
                .SetProjectFile(Solution.Starscript_Net));

            Log.Information("NuGet package written to '{packagePath}'", NuGetPackageOutput);
        });

    Target Ci => _ => _
        .Unlisted()
        .Requires(() => Host is GitHubActions)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(_ => _
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsPath)
                .SetProjectFile(Solution.Starscript_Net)
                .SetVersion(NextVersion)
                .SetVersionPrefix(NextVersion)
                .SetVersionSuffix(GitTasks.GitCurrentCommit()));

            DotNetTasks.DotNetNuGetPush(_ => _
                .SetTargetPath(NuGetPackageOutput)
                .SetSource("nuget.org")
                .SetApiKey(NugetApiKey.NotNull()));
        });
}