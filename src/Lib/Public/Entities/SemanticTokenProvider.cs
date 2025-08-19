using Starscript.Internal;

namespace Starscript;

public partial record struct SemanticToken
{
    /// <summary>
    ///     Provides a list of <see cref="SemanticToken"/>s for a given Starscript source input. The main use case for semantic tokens is syntax highlighting.
    /// </summary>
    /// <param name="source">The Starscript source.</param>
    /// <param name="tokens">Pre-allocated mutable list of <see cref="SemanticToken"/>s. The first operation of this method clears this list. All resolved <see cref="SemanticToken"/>s are then added to it.</param>
    public static void Provide(string source, List<SemanticToken> tokens)
    {
        tokens.Clear();

        var lexer = new Lexer(source);
        lexer.Next();

        while (lexer.Token != Token.EOF)
        {
            switch (lexer.Token)
            {
                case Token.EqualEqual:
                case Token.BangEqual:
                case Token.Greater:
                case Token.GreaterEqual:
                case Token.Less:
                case Token.LessEqual:
                case Token.LeftShift: 
                case Token.RightShift:
                case Token.Plus:
                case Token.Minus:
                case Token.Star:
                case Token.Slash:
                case Token.Percentage:
                case Token.UpArrow:
                case Token.Bang:
                case Token.QuestionMark:
                case Token.Colon:
                    tokens.Add(new SemanticToken(SemanticTokenType.Operator, lexer.Start, lexer.Current));
                    break;
                
                case Token.Null:
                case Token.True:
                case Token.False:
                case Token.And:
                case Token.Or:
                    tokens.Add(new SemanticToken(SemanticTokenType.Keyword, lexer.Start, lexer.Current));
                    break;
                
                case Token.String:
                    if (lexer.IsInExpression) 
                        tokens.Add(new SemanticToken(SemanticTokenType.String, lexer.Start, lexer.Current));
                    break;

                case Token.Number:
                    tokens.Add(new SemanticToken(SemanticTokenType.Number, lexer.Start, lexer.Current));
                    break;
                
                case Token.LeftParen:
                case Token.RightParen:
                    tokens.Add(new SemanticToken(SemanticTokenType.Paren, lexer.Start, lexer.Current));
                    break;

                case Token.LeftBrace:
                case Token.RightBrace:
                    tokens.Add(new SemanticToken(SemanticTokenType.Brace, lexer.Start, lexer.Current));
                    break;

                case Token.Section:
                    tokens.Add(new SemanticToken(SemanticTokenType.Section, lexer.Start, lexer.Current));
                    break;
                
                case Token.Dot:
                    tokens.Add(new SemanticToken(SemanticTokenType.Dot, lexer.Start, lexer.Current));
                    break;
                
                case Token.Comma:
                    tokens.Add(new SemanticToken(SemanticTokenType.Comma, lexer.Start, lexer.Current));
                    break;
            }
            
            lexer.Next();
        }

        // Parser
        var result = Parser.Parse(source);

        if (result.HasErrors)
        {
            var error = result.Errors.First();

            List<int> indicesToRemove = [];
            
            // Remove tokens at the same position or after the error

            foreach (var (idx, token) in tokens.Index())
                if (token.End > error.CharacterPos)
                    indicesToRemove.Add(idx);
            
            foreach (var index in indicesToRemove)
                tokens.RemoveAt(index);

            // Add the error token starting at the error position going to the end of the source
            tokens.Add(new SemanticToken(SemanticTokenType.Error, error.CharacterPos, source.Length));
        }
        else 
            result.Accept(new Visitor(tokens));
        

        tokens.Sort(Comparer<SemanticToken>.Create((x, y) => x.Start.CompareTo(y.Start)));
    }

    private class Visitor : AbstractExprVisitor
    {
        private readonly List<SemanticToken> _tokens;
        
        public Visitor(List<SemanticToken> tokens)
        {
            _tokens = tokens;
        }

        public override void Visit(Expr.Variable expr)
        {
            if (expr.Parent is not Expr.Get)
                _tokens.Add(new SemanticToken(SemanticTokenType.Identifier, expr.End - expr.Name.Length, expr.End));
            
            base.Visit(expr);
        }

        public override void Visit(Expr.Get expr)
        {
            switch (expr.Object)
            {
                case Expr.Variable:
                    _tokens.Add(new SemanticToken(SemanticTokenType.Map, expr.Start, expr.End));
                    break;
                case Expr.Get getExpr:
                    _tokens.Add(new SemanticToken(SemanticTokenType.Map, getExpr.End - getExpr.Name.Length, expr.End));
                    break;
            }

            if (expr.Parent is not Expr.Get)
                _tokens.Add(new SemanticToken(SemanticTokenType.Identifier, expr.End - expr.Name.Length, expr.End));

            base.Visit(expr);
        }
    }
}