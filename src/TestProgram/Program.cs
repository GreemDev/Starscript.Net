using Starscript;
using Starscript.Internal;
using Starscript.Util;

string source = "Name: {user.name}     Age: {user.age()}";

Console.WriteLine("Input: ");
Console.WriteLine(source);

DebugLogger.AllOutput = true;

Script script = Compiler.CompileFromSource(source);

var hypervisor = StarscriptHypervisor.CreateWithStdLib().WithStandardLibraryEnv();

hypervisor.Set("user.name", "GreemDev");
hypervisor.Set("user.age", _ => 5);

Console.WriteLine("Output: ");
Console.WriteLine("    " + hypervisor.Run(script));

hypervisor.Remove("user.name", out _);

Console.WriteLine("Output #2: ");
Console.WriteLine("    " + hypervisor.Run(script));