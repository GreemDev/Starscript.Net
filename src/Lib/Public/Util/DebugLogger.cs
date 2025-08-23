using System.Runtime.CompilerServices;

namespace Starscript.Util;

#if DEBUG

public enum DebugLogSource
{
    Lexer, 
    Parser, 
    Compiler, 
    Hypervisor,
    Test
}

public static class DebugLogger
{
    public static bool AllOutput
    {
        get => LexerOutput && ParserOutput && CompilerOutput && HypervisorOutput;
        set => LexerOutput = ParserOutput = CompilerOutput = HypervisorOutput = value;
    }
    
    public static bool LexerOutput { get; set; } = false;
    public static bool ParserOutput { get; set; } = false;
    public static bool CompilerOutput { get; set; } = false;
    public static bool HypervisorOutput { get; set; } = false;
    
    public static void Print(DebugLogSource source, string message,
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default,
        [CallerMemberName] string callerName = default!)
        // ReSharper disable ExplicitCallerInfoArgument
        => Print(source, message, InvocationInfo.Here(sourceLocation, lineNumber, callerName));
    
    public static void Print(DebugLogSource source, string message, InvocationInfo invocation = default)
    {
        if (!invocation.IsInitialized)
            Console.WriteLine($"[{Enum.GetName(source)}] {message}");
        else
        {
            var invocationInfo = invocation.ToString();
            Console.WriteLine( $"{invocationInfo,-PaddingWidth}> [{Enum.GetName(source)}] {message}");
        }
    }
    
    public const int PaddingWidth = 55;
}
    
public readonly struct InvocationInfo
{
    /// <summary>
    ///     Creates an <see cref="InvocationInfo"/> with information about the current source file, line, and member name.
    ///     Do not provide the arguments!
    /// </summary>
    /// <remarks>Mostly used in the logger.</remarks>
    /// <returns>An <see cref="InvocationInfo"/> referencing the specific line in the specific member, in the source file in which it is created.</returns>
    public static InvocationInfo Here(
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default,
        [CallerMemberName] string callerName = default!
    ) => new(sourceLocation, lineNumber, callerName);

    /// <summary>
    ///     Creates a partial <see cref="InvocationInfo"/> with information about the current source file and line.
    ///     Do not provide the arguments!
    /// </summary>
    /// <remarks>Mostly used in the logger.</remarks>
    /// <returns>An <see cref="InvocationInfo"/> referencing the specific line in the source file in which it is created.</returns>
    public static InvocationInfo CurrentFileLocation(
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default
    ) => new(sourceLocation, lineNumber);

    /// <summary>
    ///     Creates a partial <see cref="InvocationInfo"/> with only information about the current member name.
    ///     Do not provide the arguments!
    /// </summary>
    /// <remarks>Mostly used in the logger.</remarks>
    /// <returns>An <see cref="InvocationInfo"/> referencing the specific C# source member it was created in.</returns>
    public static InvocationInfo CurrentMember([CallerMemberName] string callerName = default!) => new(callerName);

    public bool IsInitialized { get; }

    public string FilePath { get; }
    public int LineInFile { get; }
    public string CallerName { get; }

    public (bool Full, bool FileLoc, bool CallerOnly) Type { get; }

    // ReSharper disable once UnusedMember.Global
    // this is used by the default keyword
    public InvocationInfo()
    {
        IsInitialized = false;
        Type = (false, false, false);
    }

    public InvocationInfo(string filePath, int line, string caller)
    {
        IsInitialized = true;
        Type = (true, false, false);

        FilePath = filePath;
        LineInFile = line;
        CallerName = caller;
    }

    public InvocationInfo(string filePath, int line)
    {
        IsInitialized = true;
        Type = (false, true, false);

        FilePath = filePath;
        LineInFile = line;
    }

    public InvocationInfo(string caller)
    {
        IsInitialized = true;
        Type = (false, false, true);

        CallerName = caller;
    }

    public override string ToString()
        => Type switch
        {
            { Full: true } => Pad(),
            { CallerOnly: true } => CallerName.PadRight(DebugLogger.PaddingWidth - CallerName.Length),
            { FileLoc: true } => $"{this.GetSourceFileName()}:{LineInFile}",
            _ => null!
        };

    private string Pad()
    {
        var paddedCaller = CallerName.PadRight(15);
        var concatenatedResult = $"{paddedCaller} > {this.GetSourceFileName()}:{LineInFile}";

        return concatenatedResult;
    }


}

public static class InvocationInfoExt
{
    public static string GetSourceFileName(this InvocationInfo invocation)
        => invocation.FilePath[
            (invocation.FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..
        ];
}
    
#endif