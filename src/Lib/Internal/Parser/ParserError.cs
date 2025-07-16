namespace Starscript.Internal;

public class ParserError
{
    
    public int Line { get; internal set; }
    public int CharacterPos { get; internal set; }
    public char Character { get; internal set; }
    public string Message { get; internal set; }
    
    public Expr? Expr { get; internal set; }

    public ParserError(int line, int characterPos, char character, string message, Expr? expr)
    {
        Line = line;
        CharacterPos = characterPos;
        Character = character;
        Message = message;
        Expr = expr;
    }
    
    public override string ToString() => $"[line {Line}, character {CharacterPos}] at '{Character}': {Message}";
}