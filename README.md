# Starscript.Net

[![NuGet Version](https://img.shields.io/nuget/v/Starscript.Net)](https://www.nuget.org/packages/Starscript.Net/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Starscript.Net)](https://www.nuget.org/packages/Starscript.Net/)

Fast text formatting language for C#. This is a .NET port of [starscript by MeteorDevelopment](https://github.com/MeteorDevelopment/starscript/), written in Java.
Licensed under MIT.

- Lightweight; No dependencies. Just .NET 9.
- Standard operators: + - * / % ^
- Ability to call functions defined in C#
- Variables can be different each time they are used
- Conditional output (ternary operator)
- Variables can be maps

## Examples
- `Hello {name}!`
- `Number: {someNumber * 100}`
- `FPS: {round(fps)}`
- `Today is a {good ? 'good' : 'bad'} day`
- `Name: {user.name}`

## Usage:
You can find the latest version number at the top of this README. With that:

Your .csproj:
```xml
<PackageReference Include="Starscript.Net" Version="{version}" />
```


In your code:
```csharp
using Starscript;

// Parse
if (!Parser.TryParse(@"Hello {name}! Starscript was originally created by {originalAuthor()}", out Parser.Result result)) 
{
    foreach (var error in result.Errors)
        Console.WriteLine(error);
    
    return;
}

// Compile
Script script = Compiler.SingleCompile(result); 
// The compiler also has the ability to batch compile and directly compile from source (throwing a ParseException if any errors are present)

// Create Starscript hypervisor instance
StarscriptHypervisor hv = StarscriptHypervisor.Create()
    .WithStandardLibrary(); // Adds the default functions, not required

hv.Set("name", () => "GreemDev"); // dynamic variable
hv.Set("originalAuthor", _ => "MineGame159"); // function

// Run
Console.WriteLine(hv.Run(script));
// or...
Console.WriteLine(script.Execute(hv));
```
