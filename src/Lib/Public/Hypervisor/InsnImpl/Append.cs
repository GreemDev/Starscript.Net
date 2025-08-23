using System.Text;
using Starscript.Internal;

namespace Starscript;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class StarscriptHypervisor
{
    protected override void Append(ref StringBuilder sb) => AppendValue(sb, Pop());

    protected override void ConstantAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr) 
        => AppendValue(sb, script.Constants[script.GetMasked(insnPtr++)]);

    protected override void VariableAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr)
    {
        var name = script.Constants[script.GetMasked(insnPtr++)].GetString();

        AppendValue(sb, GetVariable(name));
    }
    
    protected override void GetAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr)
    {
        var name = script.Constants[script.GetMasked(insnPtr++)].GetString();

        var value = Pop();

        AppendValue(sb, value.IsMap
            ? value.GetMap().GetRaw(name)?.Invoke()
            : null);
    }

    protected override void CallAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr)
    {
        var argCount = script[insnPtr++];

        var a = Peek(argCount);

        if (a.IsNull)
            throw Error("Unknown function.");

        if (!a.IsFunction)
            throw Error("Tried to call {0}, can only call functions.", Enum.GetName(a.Type));

        var result = a.GetFunction()(this, argCount);
        Pop();
        AppendValue(sb, result);
    }

    protected override void VariableGetAppend(ref StringBuilder sb, ref ExecutableScript script, ref int insnPtr)
    {
        Value? value;

        {
            // Variable
            var name = script.Constants[script.GetMasked(insnPtr++)].GetString();
            value = GetVariable(name) ?? Value.Null;
        }

        {
            // Get
            var name = script.Constants[script.GetMasked(insnPtr++)].GetString();

            if (!value.IsMap)
            {
                Push(null);
                return;
            }

            value = value.GetMap().GetRaw(name)?.Invoke();
        }

        AppendValue(sb, value);
    }
}