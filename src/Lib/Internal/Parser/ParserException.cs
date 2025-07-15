namespace Starscript.Internal;

public class ParserException : FormatException
{
    public ParserError Error { get; }
    
    public ParserException(ParserError err, string? message = null) : base(message ?? err.ToString())
    {
        Error = err;
    }
}