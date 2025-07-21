using Starscript;
using Starscript.Util;

public static class Program
{
    internal static void Main()
    {
        string source = "Name: {user.name}     Age: {user.age()}";

        Console.WriteLine("Input: ");
        Console.WriteLine(source);

#if DEBUG
        DebugLogger.AllOutput = true;
#endif

        if (!StarscriptFactory.TryCompile(source, out Script script, Console.WriteLine))
            return;

        var hypervisor = StarscriptHypervisor.CreateWithStdLib(@unsafe: true);

        hypervisor.Set("user", new TestUser());

        Console.WriteLine("Output: ");
        Console.WriteLine("    " + script.Execute(hypervisor));

        hypervisor.Remove("user.name", out _);

        Console.WriteLine("Output #2: ");
        Console.WriteLine("    " + script.Execute(hypervisor));
    }
}

struct TestUser : IStarscriptObject<TestUser>
{
    public ValueMap ToStarscript() => new ValueMap()
        .Set("name", "GreemDev")
        .Set("age", _ => 5);
}

