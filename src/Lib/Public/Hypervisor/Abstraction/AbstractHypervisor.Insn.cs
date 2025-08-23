using System.Text;
using Starscript.Internal;

namespace Starscript.Abstraction;

public partial class AbstractHypervisor<TSelf>
{
    protected abstract void Jump(ref ExecutableScript script, ref int insnPtr);
    protected abstract void JumpIfTrue(ref ExecutableScript script, ref int insnPtr);
    protected abstract void JumpIfFalse(ref ExecutableScript script, ref int insnPtr);
    protected abstract void Not();
    protected abstract void Equals();
    protected abstract void NotEquals();
    protected abstract void Greater();
    protected abstract void GreaterEqual();
    protected abstract void Less();
    protected abstract void LessEqual();
    
    protected abstract void Add();
    protected abstract void Negate();
    protected abstract void Subtract();
    protected abstract void Multiply();
    protected abstract void Divide();
    protected abstract void Modulo();
    protected abstract void Power();
    protected abstract void RightShift();
    protected abstract void LeftShift();

    protected abstract void Section(
        ref StringBuilder sb,
        ref ExecutableScript script,
        ref StringSegment firstSegment,
        ref StringSegment segment, 
        ref int index, 
        ref int insnPtr);

    protected abstract void Append(ref StringBuilder sb);
    protected abstract void ConstantAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);
    protected abstract void VariableAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);
    protected abstract void GetAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);
    protected abstract void CallAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);
    protected abstract void VariableGetAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr);
    protected abstract void AddConstant(ref ExecutableScript script, ref int insnPtr);
    protected abstract void Constant(ref ExecutableScript script, ref int insnPtr);
    protected abstract void Variable(ref ExecutableScript script, ref int insnPtr);
    protected abstract void Get(ref ExecutableScript script, ref int insnPtr);
    protected abstract void VariableGet(ref ExecutableScript script, ref int insnPtr);
    protected abstract void Call(ref ExecutableScript script, ref int insnPtr);
    
    protected abstract StringSegment EndExecution(ref StringBuilder sb, ref StringSegment firstSegment,
        ref StringSegment segment, int index);
}