using Starscript.Internal;

namespace Starscript;

public static class StarscriptFactory
{
    /// <summary>
    ///     Runs the <paramref name="source"/> string through the Starscript <see cref="Parser"/>, directly returning the result. 
    /// </summary>
    public static ParserResult Parse(string source) => Parser.Parse(source);

    /// <summary>
    ///     Tries to compile the provided <paramref name="source"/> string into an executable Starscript <see cref="Script"/>.
    /// </summary>
    /// <param name="source">Starscript</param>
    /// <param name="compiledScript">Compiled executable Starscript output.</param>
    /// <param name="onError">The delegate to call with the provided error.</param>
    /// <returns>true, if the script has been compiled; false otherwise.</returns>
    public static bool TryCompile(string source, out Script compiledScript, Action<ParserError>? onError = null)
    {
        var result = Parse(source);

        if (result.HasErrors)
        {
            compiledScript = null!;
            onError?.Invoke(result.Errors.First());
        }
        else
        {
            compiledScript = Compile(result);
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return compiledScript != null;
    }

    /// <summary>
    ///     Compiles a parser result into a Starscript <see cref="Script"/> capable of being executed by <see cref="StarscriptHypervisor"/>.
    ///     <br/>
    ///     This method assumes the caller has already checked for errors in the <see cref="ParserResult"/>.
    /// </summary>
    /// <returns>Execution-ready compiled Starscript</returns>
    public static Script Compile(ParserResult result) => Compiler.Compile(result);
}