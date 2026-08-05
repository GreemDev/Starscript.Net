using Nuke.Common.CI.GitHubActions;

[GitHubActions("ci",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranches = ["main"],
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