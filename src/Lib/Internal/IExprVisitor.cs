namespace Starscript.Internal;

public interface IExprVisitor
{
    void Visit(Expr.Null expr);
    void Visit(Expr.String expr);
    void Visit(Expr.Number expr);
    void Visit(Expr.Boolean expr);
    void Visit(Expr.Block expr);
    void Visit(Expr.Group expr);
    void Visit(Expr.Binary expr);
    void Visit(Expr.Unary expr);
    void Visit(Expr.Variable expr);
    void Visit(Expr.Get expr);
    void Visit(Expr.Call expr);
    void Visit(Expr.Logical expr);
    void Visit(Expr.Conditional expr);
    void Visit(Expr.Section expr);
}