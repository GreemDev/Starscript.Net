using Starscript.Internal;

namespace Starscript;

public class AbstractExprVisitor : Expr.IVisitor
{
    private void VisitAll<TExpr>(TExpr expr) where TExpr : Expr
    {
        foreach (var childExpr in expr.Children) 
            childExpr.Accept(this);
    }
    
    public virtual void Visit(Expr.Null expr) => VisitAll(expr);
    public virtual void Visit(Expr.String expr) => VisitAll(expr);
    public virtual void Visit(Expr.Number expr) => VisitAll(expr);
    public virtual void Visit(Expr.Boolean expr) => VisitAll(expr);
    public virtual void Visit(Expr.Block expr) => VisitAll(expr);
    public virtual void Visit(Expr.Group expr) => VisitAll(expr);
    public virtual void Visit(Expr.Binary expr) => VisitAll(expr);
    public virtual void Visit(Expr.Unary expr) => VisitAll(expr);
    public virtual void Visit(Expr.Variable expr) => VisitAll(expr);
    public virtual void Visit(Expr.Get expr) => VisitAll(expr);
    public virtual void Visit(Expr.Call expr) => VisitAll(expr);
    public virtual void Visit(Expr.Logical expr) => VisitAll(expr);
    public virtual void Visit(Expr.Conditional expr) => VisitAll(expr);
    public virtual void Visit(Expr.Section expr) => VisitAll(expr);
}