using Starscript.Internal;

namespace Starscript;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class StarscriptHypervisor
{
    protected virtual void Jump(ref ExecutableScript script, ref int insnPtr)
    {
        var jump = (script.GetMasked(insnPtr++) << 8)
                   | script.GetMasked(insnPtr++);

        insnPtr += jump;
    }

    protected virtual void JumpIfTrue(ref ExecutableScript script, ref int insnPtr)
    {
        var jump = (script.GetMasked(insnPtr++) << 8)
                   | script.GetMasked(insnPtr++);

        if (Peek().IsTruthy)
            insnPtr += jump;
    }

    protected virtual void JumpIfFalse(ref ExecutableScript script, ref int insnPtr)
    {
        var jump = (script.GetMasked(insnPtr++) << 8)
                   | script.GetMasked(insnPtr++);

        if (!Peek().IsTruthy)
            insnPtr += jump;
    }

    protected virtual void Not() => Push(!Pop().IsTruthy);

    protected virtual void Equals()
    {
        // ReSharper disable once EqualExpressionComparison
        Push(Pop().Equals(Pop()));
    }

    protected virtual void NotEquals()
    {
        // ReSharper disable once EqualExpressionComparison
        Push(!Pop().Equals(Pop()));
    }

    protected virtual void Greater()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() > b.GetNumber());
        else
            throw Error("> operation requires 2 numbers.");
    }

    protected virtual void GreaterEqual()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() >= b.GetNumber());
        else
            throw Error(">= operation requires 2 numbers.");
    }

    protected virtual void Less()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() < b.GetNumber());
        else
            throw Error("< operation requires 2 numbers.");
    }

    protected virtual void LessEqual()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() <= b.GetNumber());
        else
            throw Error("<= operation requires 2 numbers.");
    }
}