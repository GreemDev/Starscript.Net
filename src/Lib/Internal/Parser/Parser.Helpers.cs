using System.Runtime.CompilerServices;
using Starscript.Internal;
using Starscript.Util;

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
        
#if DEBUG
        DebugLog($"Next token data created: " +
                 $"Token {_lexer.Token}, Lexeme '{_lexer.Lexeme}', " +
                 $"Start {_lexer.Start}, End {_lexer.Current}, Line {_lexer.Line}, " +
                 $"CharPos {_lexer.Character}, Char '{_lexer.CurrentChar}'");
#endif

        return ref _previous;
    }

#if DEBUG
    private void DebugLog(string message,
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default,
        [CallerMemberName] string callerName = default!)
    {
        if (DebugLogger.ParserOutput)
            // ReSharper disable ExplicitCallerInfoArgument
            DebugLogger.Print(DebugLogSource.Parser, message, InvocationInfo.Here(sourceLocation, lineNumber, callerName));
    }
#endif
}