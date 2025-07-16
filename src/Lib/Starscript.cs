using Starscript.Internal;

namespace Starscript;

public class Starscript
{
    public static ParserResult Parse(string script) => new Parser(script).Run();
}