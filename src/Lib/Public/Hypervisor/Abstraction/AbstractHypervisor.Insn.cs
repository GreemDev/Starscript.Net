using System.Runtime.CompilerServices;
using System.Text;
using Starscript.Internal;

namespace Starscript.Abstraction;

public partial class AbstractHypervisor<TSelf>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Jump(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void JumpIfTrue(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void JumpIfFalse(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Not();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Equals();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void NotEquals();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Greater();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void GreaterEqual();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Less();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void LessEqual();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Add();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Negate();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Subtract();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Multiply();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Divide();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Modulo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Power();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void LeftShift();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void RightShift();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void UnsignedRightShift();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void BitwiseAnd();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void BitwiseOr();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void BitwiseXor();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void BitwiseNot();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Section(
        ref StringBuilder sb,
        ref ExecutableScript script,
        ref StringSegment firstSegment,
        ref StringSegment segment,
        ref int index,
        ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Append(ref StringBuilder sb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void ConstantAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void VariableAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void GetAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void CallAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void VariableGetAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void AddConstant(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Constant(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Variable(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Get(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void VariableGet(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void Call(ref ExecutableScript script, ref int insnPtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract StringSegment EndExecution(ref StringBuilder sb, ref StringSegment firstSegment,
        ref StringSegment segment, int index);
}