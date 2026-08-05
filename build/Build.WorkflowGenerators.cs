using Nuke.Common.CI.GitHubActions;

[GitHubActions("main",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranches = ["main"],
    OnPushIncludePaths = [
        "Starscript.Net.sln",
        "build/**",
        "src/Lib/**"
    ],
    OnPushExcludePaths =
    [
        ".github/**",
        "docs/**",
        "assets/**",
        ".vscode/**",
        "*.yml",
        "*.json",
        "*.md",
        ".gitignore",
        ".gitattributes"
    ], InvokedTargets = [nameof(Ci)],
    EnableGitHubToken = true,
    ImportSecrets = [nameof(NugetApiKey)])]
[GitHubActions("benchmarks",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPullRequestBranches = ["main"],
    OnPullRequestIncludePaths = [
        "Starscript.Net.sln",
        "build/**",
        "src/Lib/**",
        "src/Benchmarks/**"
    ],
    OnPullRequestExcludePaths =
    [
        ".github/**",
        "docs/**",
        "assets/**",
        ".vscode/**",
        "*.yml",
        "*.json",
        "*.md",
        ".gitignore",
        ".gitattributes"
    ],
    OnPushBranches = ["main"],
    OnPushIncludePaths = [
        "Starscript.Net.sln",
        "build/**",
        "src/Lib/**",
        "src/Benchmarks/**"
    ],
    OnPushExcludePaths =
    [
        ".github/**",
        "docs/**",
        "assets/**",
        ".vscode/**",
        "*.yml",
        "*.json",
        "*.md",
        ".gitignore",
        ".gitattributes"
    ], InvokedTargets = [nameof(Benchmarks)])]
partial class Build;