using Starscript.Internal;

namespace Starscript;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class StarscriptHypervisor
{
    protected override void AddConstant(ref ExecutableScript script, ref int insnPtr)
    {
        var b = script.Constants[script.GetMasked(insnPtr++)];
        var a = Pop();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() + b.GetNumber());
        else if (a.IsString)
            Push(a.GetString() + b);
        else
            throw Error("Can only add 2 numbers, or 1 string and any other value.");
    }

    protected override void Constant(ref ExecutableScript script, ref int insnPtr)
        => Push(script.Constants[script.GetMasked(insnPtr++)]);
}