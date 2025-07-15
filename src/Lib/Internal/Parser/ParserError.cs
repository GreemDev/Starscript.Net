namespace Starscript.Internal;

public readonly record struct ParserError(int Line, int CharacterPos, char Character, string Message, Expr Expr)
{
    public override string ToString() => $"[line {Line}, character {CharacterPos}] at '{Character}': {Message}";
}