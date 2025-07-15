namespace Starscript.Internal;

public interface IExprVisitor :
    IExprVisitor<Expr.Null>,
    IExprVisitor<Expr.String>,
    IExprVisitor<Expr.Number>,
    IExprVisitor<Expr.Boolean>,
    IExprVisitor<Expr.Block>,
    IExprVisitor<Expr.Group>,
    IExprVisitor<Expr.Binary>,
    IExprVisitor<Expr.Unary>,
    IExprVisitor<Expr.Variable>,
    IExprVisitor<Expr.Get>,
    IExprVisitor<Expr.Call>,
    IExprVisitor<Expr.Logical>,
    IExprVisitor<Expr.Conditional>,
    IExprVisitor<Expr.Section>;

public interface IExprVisitor<in TExpr> where TExpr : Expr
{
    void Visit(TExpr expr);
}