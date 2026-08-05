// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Starscript.Net.Benchmarks;

if (args.Contains("--ci"))
{
    BenchmarkRunner.Run<StarscriptVsStringFormat>(config: DefaultConfig.Instance
        .WithArtifactsPath("benchout/")
    );
}
else
{
    BenchmarkRunner.Run<StarscriptVsStringFormat>();
}