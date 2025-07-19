using Starscript;
using Starscript.Util;

string source = "Name: {user.name}     Age: {user.age()}";

Console.WriteLine("Input: ");
Console.WriteLine(source);

#if DEBUG
DebugLogger.AllOutput = true;
#endif

if (!StarscriptFactory.TryCompile(source, out Script script, Console.WriteLine))
    return;

var hypervisor = StarscriptHypervisor.CreateWithStdLib(@unsafe: true);

hypervisor.Set("user.name", "GreemDev");
hypervisor.Set("user.age", _ => 5);

Console.WriteLine("Output: ");
Console.WriteLine("    " + script.Execute(hypervisor));

hypervisor.Remove("user.name", out _);

Console.WriteLine("Output #2: ");
Console.WriteLine("    " + script.Execute(hypervisor));