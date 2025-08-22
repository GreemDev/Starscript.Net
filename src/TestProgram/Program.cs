using System.Runtime.CompilerServices;
using Starscript;
using Starscript.Util;

public static class Program
{
#if DEBUG

    private static void Log(string msg,
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default,
        [CallerMemberName] string callerName = default!)
    {
        DebugLogger.Print(DebugLogSource.Test, msg, sourceLocation, lineNumber, callerName);
    }

#endif

    internal static void Main()
    {
#if !DEBUG
        throw new InvalidOperationException(
            "TestProgram can only be run in debug as it relies on debug-only logging instrumentation in Starscript.");
#else

        string source = "Name: {name}     Age: {age() >> 2}";

        DebugLogger.AllOutput = true;

        if (!StarscriptFactory.TryCompile(source, out Script script, Console.WriteLine))
            return;

        var hypervisor = StarscriptHypervisor.Create().WithStandardLibrary();
        var segment = script.Execute(hypervisor, new TestUser());

        Log("Input: ");
        Log("    " + source);

        Log("Output: ");
        Log("    " + segment);

        Log("Walked: {");
        segment.Walk(seg =>
            Log($"    Index: {seg.Index}, Content: \"{seg.Content}\", HasNext: {seg.Next is not null}")
        );
        Log("}");

        try
        {
            DebugLogger.HypervisorOutput = false;

            script.Execute(hypervisor);

            throw new NotImplementedException();
        }
        catch (StarscriptException)
        {
            Log("Successfully errored when trying to access a cleared local variable.");

            // StarscriptHypervisor should error here, as the source string calls a function that is defined as a local, so it is cleared when the script execution ends.
            // NotImplementedException is not caught as that is an error case, that being StarscriptHypervisor *not* erroring.  
        }

        script.Dispose();
#endif
    }
}

struct TestUser : IStarscriptObject
{
    public ValueMap ToStarscript() => new ValueMap()
        .Set("name", "GreemDev")
        .Set("age", _ => 60);
}