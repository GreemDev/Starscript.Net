namespace Starscript.Internal;

public class ParseException : FormatException
{
    public ParserError Error { get; }
    
    public ParseException(ParserError err, string? message = null) : base(message ?? err.ToString())
    {
        Error = err;
    }
}