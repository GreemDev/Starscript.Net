using Starscript;
using Starscript.Util;

public static class Program
{
    internal static void Main()
    {
        string source = "Name: {name}     Age: {age() >> 2}";

        Console.WriteLine("Input: ");
        Console.WriteLine(source);

#if DEBUG
        DebugLogger.AllOutput = true;
#endif

        if (!StarscriptFactory.TryCompile(source, out Script script, Console.WriteLine))
            return;

        var hypervisor = StarscriptHypervisor.Create().WithStandardLibrary();

        Console.WriteLine("Output: ");
        Console.WriteLine("    " + script.Execute(hypervisor, new TestUser()));

        try
        {
            DebugLogger.HypervisorOutput = false;
            
            script.Execute(hypervisor);

            throw new NotImplementedException();
        }
        catch (StarscriptException)
        {
            // StarscriptHypervisor should error here, as the source string calls a function that is defined as a local, so it is cleared when the script execution ends.
            // NotImplementedException is not caught as that is an error case, that being StarscriptHypervisor *not* erroring.  
        }
        
        script.Dispose();
    }
}

struct TestUser : IStarscriptObject
{
    public ValueMap ToStarscript() => new ValueMap()
        .Set("name", "GreemDev")
        .Set("age", _ => 60);
}

