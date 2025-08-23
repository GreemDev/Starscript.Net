using Starscript.Internal;

namespace Starscript;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class StarscriptHypervisor
{
    protected override void Variable(ref ExecutableScript script, ref int insnPtr)
    {
        var name = script.Constants[script.GetMasked(insnPtr++)].GetString();

        Push(GetVariable(name));
    }

    protected override void Get(ref ExecutableScript script, ref int insnPtr)
    {
        var name = script.Constants[script.GetMasked(insnPtr++)].GetString();

        var value = Pop();

        Push(!value.IsMap
            ? Value.Null
            : GetVariable(name)
        );
    }

    protected override void VariableGet(ref ExecutableScript script, ref int insnPtr)
    {
        Value value;

        {
            // Variable
            var name = script.Constants[script.GetMasked(insnPtr++)].GetString();
            value = GetVariable(name) ?? Value.Null;
        }

        {
            // Get
            var name = script.Constants[script.GetMasked(insnPtr++)].GetString();

            Push(value.IsMap
                ? value.GetMap().GetRaw(name)?.Invoke()
                : null
            );
        }
    }

    protected override void Call(ref ExecutableScript script, ref int insnPtr)
    {
        var argCount = script[insnPtr++];

        var a = Peek(argCount);

        if (a.IsNull)
            throw Error("Unknown function.");

        if (!a.IsFunction)
            throw Error("Tried to call {0}, can only call functions.", Enum.GetName(a.Type));

        var result = a.GetFunction()(this, argCount);
        Pop();
        Push(result);
    }
}