using System.Collections.Immutable;
using Starscript.Internal;

namespace Starscript;

public partial class Compiler
{
    /// <summary>
    ///     Compile all given <see cref="ParserResult"/>s in the <see cref="IEnumerable{T}"/> into an immutable array of immutable <see cref="Script"/>s, using the same <see cref="Compiler"/> instance.
    /// </summary>
    /// <param name="parserResults">The parser results to compile.</param>
    /// <returns>An immutable array of immutable <see cref="Script"/>s.</returns>
    public static ImmutableArray<Script> CompileAll(IEnumerable<ParserResult> parserResults)
    {
        var results = ImmutableArray.CreateBuilder<Script>();

        var compiler = new Compiler();

        foreach (var parserResult in parserResults)
        {
            compiler.Compile(parserResult);
            results.Add(compiler.MoveToImmutableAndReset());
        }

        return results.MoveToImmutable();
    }

    /// <summary>
    ///     Compile all given <see cref="ParserResult"/>s in the <see cref="IEnumerable{T}"/> into an immutable array of immutable <see cref="Script"/>s, using the same <see cref="Compiler"/> instance.
    /// </summary>
    /// <param name="parserResults">The parser results to compile.</param>
    /// <returns>An immutable array of immutable <see cref="Script"/>s.</returns>
    public static ImmutableArray<Script> BatchCompile(params IEnumerable<ParserResult> parserResults)
    {
        var results = ImmutableArray.CreateBuilder<Script>();

        var compiler = new Compiler();

        foreach (var parserResult in parserResults)
        {
            compiler.Compile(parserResult);
            results.Add(compiler.MoveToImmutableAndReset());
        }

        return results.MoveToImmutable();
    }
    
    /// <summary>
    ///     Parse, then compile, the given Starscript source into an immutable <see cref="Script"/>.
    /// </summary>
    /// <param name="source">The Starscript source input.</param>
    /// <returns>An immutable <see cref="Script"/>.</returns>
    /// <exception cref="ParseException">Thrown if the internal <see cref="ParserResult"/> contains any errors.</exception>
    public static Script DirectCompile(string source)
    {
        var parsed = Parser.Parse(source);
        if (parsed.HasErrors)
            throw new ParseException(parsed.Errors.First());

        return SingleCompile(parsed);
    }
    
    /// <summary>
    ///     Compile the given <see cref="ParserResult"/> into an immutable <see cref="Script"/>.
    /// </summary>
    /// <param name="result">The <see cref="ParserResult"/> to compile.</param>
    /// <returns>An immutable <see cref="Script"/>.</returns>
    public static Script SingleCompile(ParserResult result) => new Compiler(result).MoveToImmutableAndReset();
}