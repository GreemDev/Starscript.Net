using Starscript.Internal;

namespace Starscript;

public partial class Parser
{
    private void Synchronize()
    {
        while (!IsAtEnd)
        {
            if (MatchAnyNext(Token.LeftBrace)) 
                _expressionDepth++;
            else if (MatchAnyNext(Token.RightBrace))
            {
                _expressionDepth--;
                if (_expressionDepth is 0) return;
            }
            else Advance();
        }
    }
    
    private ParseException Error(string message, Expr? expr) =>
        throw new ParseException(new ParserError(_current.Line, _current.CharacterPos, _current.Character, 
            message, expr));

    private bool MatchAnyNext(params Token[] tokens)
    {
        foreach (var token in tokens)
        {
            if (Check(token))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private ref TokenData Consume(Token token, string message, Expr expr)
    {
        if (Check(token)) return ref Advance();
        
        throw Error(message, expr);
    }
    
    private bool Check(Token token) => !IsAtEnd && _current.Token == token;
    private bool IsAtEnd => _current.Token is Token.EOF;
    
    private ref TokenData Advance()
    {
        _previous.Set(ref _current);
        
        _lexer.Next();
        _current.Set(_lexer.Token, _lexer.Lexeme, _lexer.Start, _lexer.Current, _lexer.Line, _lexer.Character, _lexer.CurrentChar);

        return ref _previous;
    }
}