using System.Text;

namespace Starscript.Internal;

/** Takes starscript source code and produces a stream or tokens that are used for parsing. */
public class Lexer {
    /** The type of the token. */
    public Token Token;
    /** The string representation of the token. */
    public string Lexeme;
    
    public int Start { get; set; }
    public int Current { get; set; }

    public int Line = 1, Character = -1;
    
    public char CurrentChar;

    private readonly string _source;
    private readonly StringBuilder _sb = new();
    private int _expressionDepth;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Lexer(string source) {
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        _source = source;
    }

    /** Scans for next token storing it in {@link Lexer#token} and {@link Lexer#lexeme}. Produces {@link Token#EOF} if the end of source code has been reached and {@link Token#Error} if there has been an error. */
    public void Next() {
        Start = Current;

        if (IsAtEnd) {
            CreateToken(Token.EOF);
            return;
        }

        if (_expressionDepth > 0) {
            // Scan expression
            SkipWhitespace();
            if (IsAtEnd) {
                CreateToken(Token.EOF);
                return;
            }

            char c = Advance();

            if (char.IsDigit(c) || (c == '-' && char.IsDigit(Peek()))) Number();
            else if (IsAlpha(c)) Identifier();
            else {
                switch (c) {
                    case '\'':
                    case '"':  String(c); break;

                    case '=':  if (Match('=')) CreateToken(Token.EqualEqual); else Unexpected(); break;
                    case '!':  CreateToken(Match('=') ? Token.BangEqual : Token.Bang); break;
                    case '>':  CreateToken(Match('=') ? Token.GreaterEqual : Token.Greater); break;
                    case '<':  CreateToken(Match('=') ? Token.LessEqual : Token.Less); break;

                    case '+':  CreateToken(Token.Plus); break;
                    case '-':  CreateToken(Token.Minus); break;
                    case '*':  CreateToken(Token.Star); break;
                    case '/':  CreateToken(Token.Slash); break;
                    case '%':  CreateToken(Token.Percentage); break;
                    case '^':  CreateToken(Token.UpArrow); break;

                    case '.':  CreateToken(Token.Dot); break;
                    case ',':  CreateToken(Token.Comma); break;
                    case '?':  CreateToken(Token.QuestionMark); break;
                    case ':':  CreateToken(Token.Colon); break;
                    case '(':  CreateToken(Token.LeftParen); break;
                    case ')':  CreateToken(Token.RightParen); break;
                    case '{':  _expressionDepth++; CreateToken(Token.LeftBrace); break;
                    case '}':  _expressionDepth--; CreateToken(Token.RightBrace); break;

                    case '#':
                        while (char.IsDigit(Peek())) Advance();
                        CreateToken(Token.Section, _source.Substring(Start + 1, Current));
                        break;

                    default:   Unexpected(); break;
                }
            }
        }
        else {
            // Scan string, start an expression or section
            char c = Advance();
            if (c == '\n') Line++;

            if (CanStartExpression(c, Peek())) {
                _expressionDepth++;
                CreateToken(Token.LeftBrace);
            }
            else if (CanStartSection(c, Peek())) {
                while (char.IsDigit(Peek())) Advance();
                CreateToken(Token.Section, _source.Substring(Start + 1, Current));
            }
            else {
                while (!IsAtEnd && !CanStartExpression(Peek(), PeekNext()) && !CanStartSection(Peek(), PeekNext())) {
                    if (Peek() == '\n') Line++;

                    char advanced = Advance();

                    if ((advanced == '{' && Peek() == '{') || (advanced == '#' && Peek() == '#')) {
                        Advance();
                    }
                }

                CreateToken(Token.String);
            }
        }
    }

    private void String(char delimiter) {
        _sb.Length = 0;

        while (!IsAtEnd) {
            if (Peek() == '\\') {
                Advance();
                if (IsAtEnd) {
                    CreateToken(Token.Error, "Unterminated expression.");
                }
            } else if (Peek() == delimiter) {
                break;
            } else if (Peek() == '\n')  {
                Line++;
            }

            _sb.Append(Advance());
        }

        if (IsAtEnd) {
            CreateToken(Token.Error, "Unterminated expression.");
        }
        else {
            Advance();
            CreateToken(Token.String, _sb.ToString());
        }
    }

    private void Number() {
        while (char.IsDigit(Peek())) Advance();

        if (Peek() == '.' && char.IsDigit(PeekNext())) {
            Advance();

            while (char.IsDigit(Peek())) Advance();
        }

        CreateToken(Token.Number);
    }

    private void Identifier() {
        while (!IsAtEnd && IsAlphanumeric(Peek())) Advance();

        CreateToken(Token.Identifier);

        switch (Lexeme) {
            case "null":  Token = Token.Null; break;
            case "true":  Token = Token.True; break;
            case "false": Token = Token.False; break;
            case "and":   Token = Token.And; break;
            case "or":    Token = Token.Or; break;
        }
    }

    private bool CanStartExpression(char c1, char c2) {
        return c1 == '{' && c2 != '{';
    }

    private bool CanStartSection(char c1, char c2) => c1 == '#' && char.IsDigit(c2);

    private void SkipWhitespace() {
        while (true) {
            if (IsAtEnd) return;
            char c = Peek();

            switch (c) {
                case ' ':
                case '\r':
                case '\t': Advance(); break;
                case '\n': Line++; Advance(); break;
                default:   Start = Current; return;
            }
        }
    }

    // Helpers

    public bool IsInExpression => _expressionDepth > 0;

    private void Unexpected() {
        CreateToken(Token.Error, "Unexpected character.");
    }

    private void CreateToken(Token token, String lexeme) {
        Token = token;
        Lexeme = lexeme;
    }

    private void CreateToken(Token token) {
        CreateToken(token, _source.Substring(Start, Current));
    }

    private bool Match(char expected) {
        if (IsAtEnd) return false;
        if (_source.ElementAt(Current) != expected) return false;

        Advance();
        return true;
    }

    private char Advance() {
        Character++;
        return CurrentChar = _source.ElementAt(Current++);
    }

    private char Peek() 
        => IsAtEnd 
            ? '\0' 
            : _source.ElementAt(Current);

    private char PeekNext() 
        => Current + 1 >= _source.Length 
            ? '\0' 
            : _source.ElementAt(Current + 1);

    private bool IsAtEnd => Current >= _source.Length;

    private bool IsAlpha(char c) {
        return c is >= 'a' and <= 'z' || c is >= 'A' and <= 'Z' || c == '_';
    }

    private bool IsAlphanumeric(char c) {
        return IsAlpha(c) || char.IsDigit(c);
    }
}