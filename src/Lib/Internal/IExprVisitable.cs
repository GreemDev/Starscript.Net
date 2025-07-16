namespace Starscript.Internal;

public interface IExprVisitable
{
    public void Accept(IExprVisitor visitor);
}