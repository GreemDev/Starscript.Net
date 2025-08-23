using Starscript.Internal;

namespace Starscript;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class StarscriptHypervisor
{
    protected override void Jump(ref ExecutableScript script, ref int insnPtr)
    {
        var jump = (script.GetMasked(insnPtr++) << 8)
                   | script.GetMasked(insnPtr++);

        insnPtr += jump;
    }

    protected override void JumpIfTrue(ref ExecutableScript script, ref int insnPtr)
    {
        var jump = (script.GetMasked(insnPtr++) << 8)
                   | script.GetMasked(insnPtr++);

        if (Peek().IsTruthy)
            insnPtr += jump;
    }

    protected override void JumpIfFalse(ref ExecutableScript script, ref int insnPtr)
    {
        var jump = (script.GetMasked(insnPtr++) << 8)
                   | script.GetMasked(insnPtr++);

        if (!Peek().IsTruthy)
            insnPtr += jump;
    }

    protected override void Not() => Push(!Pop().IsTruthy);

    protected override void Equals()
    {
        // ReSharper disable once EqualExpressionComparison
        Push(Pop().Equals(Pop()));
    }

    protected override void NotEquals()
    {
        // ReSharper disable once EqualExpressionComparison
        Push(!Pop().Equals(Pop()));
    }

    protected override void Greater()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() > b.GetNumber());
        else
            throw Error("> operation requires 2 numbers.");
    }

    protected override void GreaterEqual()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() >= b.GetNumber());
        else
            throw Error(">= operation requires 2 numbers.");
    }

    protected override void Less()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() < b.GetNumber());
        else
            throw Error("< operation requires 2 numbers.");
    }

    protected override void LessEqual()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() <= b.GetNumber());
        else
            throw Error("<= operation requires 2 numbers.");
    }
}