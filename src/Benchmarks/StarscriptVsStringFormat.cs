using BenchmarkDotNet.Attributes;

namespace Starscript.Net.Benchmarks;

public class StarscriptVsStringFormat
{
    public static StarscriptHypervisor Hypervisor { get; } = StarscriptHypervisor.Create().WithStandardLibrary();

    public const string FormatSource = "FPS: {0}";
    public const string StarscriptSource = "FPS: {round(fps)}";

    public static readonly Script PrecompiledTestScript = Compiler.DirectCompile(StarscriptSource);

    public StarscriptVsStringFormat()
    {
        Hypervisor.Set("fps", 59.68223);
    }

    [Benchmark]
    public void StringFormat()
    {
        _ = string.Format(FormatSource, Math.Round(59.68223));
    }
    
    [Benchmark]
    public void PrecompiledStarscript()
    {
        _ = PrecompiledTestScript.Execute(Hypervisor);
    }
    
    [Benchmark]
    public void JitStarscript()
    {
        using var script = Compiler.DirectCompile(StarscriptSource);
        
        _ = script.Execute(Hypervisor);
    }
}