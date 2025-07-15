namespace Starscript.Internal;

public record struct TokenData(
    Token Token,
    string Lexeme,
    int Start,
    int End,
    int Line,
    int CharacterPos,
    char Character)
{
    public void Set(Token token, string lexeme, int start, int end, int line, int characterPos, char character)
    {
        Token = token;
        Lexeme = lexeme;
        Start = start;
        End = end;
        Line = line;
        CharacterPos = characterPos;
        Character = character;
    }

    public void Set(TokenData other) => Set(
        other.Token, other.Lexeme, 
        other.Start, other.End, other.Line,
        other.CharacterPos, other.Character
    );
}