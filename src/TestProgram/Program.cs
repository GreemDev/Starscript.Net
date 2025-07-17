using System.Diagnostics;
using Starscript;
using Starscript.Internal;

string source = "Name: {user.name}     Age: {498474 + 48894}";

Script script = Compiler.CompileFromSource(source);

var hypervisor = new StarscriptHypervisor();

hypervisor.Set("user.name", "GreemDev");
hypervisor.Set("user.age", _ => 5);

Console.WriteLine("Input: ");
Console.WriteLine(source);
Console.WriteLine("Output: ");
Console.WriteLine("    " + hypervisor.Run(script));

hypervisor.Remove("user.name", out _);

Console.WriteLine("Output #2: ");
Console.WriteLine("    " + hypervisor.Run(script));