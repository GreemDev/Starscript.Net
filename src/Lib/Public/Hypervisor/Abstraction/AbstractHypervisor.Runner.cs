using System.Text;
using Starscript.Internal;
using Starscript.Util;

namespace Starscript.Abstraction;

public partial class AbstractHypervisor<TSelf>
{
    protected virtual StringSegment RunInternal(ExecutableScript script, StringBuilder sb)
    {
        if (script.IsDisposed)
            throw new ObjectDisposedException(script.GetType().FullName, "Cannot execute a disposed Script.");

        if (script.CodeSize is 0)
            throw new StarscriptException($"Attempted to execute a {script.GetType().Name} with no bytecode.");

        ClearStack();

        sb.Length = 0;

        int instructionPointer = 0;

        StringSegment? firstSegment = null;
        StringSegment? segment = null;
        int index = 0;

        while (true)
        {
#if DEBUG
            var idx = instructionPointer++;
            var insn = (Instruction)script[idx];
            DebugLog($"Processing {Enum.GetName(insn)} instruction @ ip 0x{idx:x8} ({idx})");

            switch (insn)
#else
            switch ((Instruction)script[instructionPointer++])
#endif
            {
                // Literals
                case Instruction.Constant: Constant(ref script, ref instructionPointer); break;
                case Instruction.Null: Push(Value.Null); break;
                case Instruction.True: Push(true); break;
                case Instruction.False: Push(false); break;

                // Arithmetic
                case Instruction.Pop: Pop(); break;
                case Instruction.Add: Add(); break;
                case Instruction.AddConstant: AddConstant(ref script, ref instructionPointer); break;
                case Instruction.Subtract: Subtract(); break;
                case Instruction.Multiply: Multiply(); break;
                case Instruction.Divide: Divide(); break;
                case Instruction.Modulo: Modulo(); break;
                case Instruction.Power: Power(); break;
                case Instruction.Negate: Negate(); break;
                case Instruction.RightShift: RightShift(); break;
                case Instruction.LeftShift: LeftShift(); break;

                // Comparison
                case Instruction.Not: Not(); break;
                case Instruction.Equals: Equals(); break;
                case Instruction.NotEquals: NotEquals(); break;
                case Instruction.Greater: Greater(); break;
                case Instruction.GreaterEqual: GreaterEqual(); break;
                case Instruction.Less: Less(); break;
                case Instruction.LessEqual: LessEqual(); break;
                case Instruction.Jump: Jump(ref script, ref instructionPointer); break;
                case Instruction.JumpIfTrue: JumpIfTrue(ref script, ref instructionPointer); break;
                case Instruction.JumpIfFalse: JumpIfFalse(ref script, ref instructionPointer); break;

                // Variable access
                case Instruction.Variable: Variable(ref script, ref instructionPointer); break;
                case Instruction.Get: Get(ref script, ref instructionPointer); break;
                case Instruction.VariableGet: VariableGet(ref script, ref instructionPointer); break;
                case Instruction.Call: Call(ref script, ref instructionPointer); break;
                
                case Instruction.Append: Append(ref sb); break;
                case Instruction.ConstantAppend: ConstantAppend(ref sb, ref script, ref instructionPointer); break;
                case Instruction.VariableAppend: VariableAppend(ref sb, ref script, ref instructionPointer); break;
                case Instruction.GetAppend: GetAppend(ref sb, ref script, ref instructionPointer); break;
                case Instruction.CallAppend: CallAppend(ref sb, ref script, ref instructionPointer); break;
                case Instruction.VariableGetAppend: 
                    VariableGetAppend(ref sb, ref script, ref instructionPointer); break;

                case Instruction.Section:
                {
                    Section(ref sb, ref script, 
                        ref firstSegment.NullableRef(), 
                        ref segment.NullableRef(), 
                        ref index, ref instructionPointer);
                    break;
                }

                case Instruction.End:
                    return EndExecution(ref sb, ref firstSegment.NullableRef(), ref segment.NullableRef(), index);
                default:
#if DEBUG
                    throw new InvalidOperationException(
                        $"Unknown instruction '{Enum.GetName(insn)}'");
#else
                    throw new InvalidOperationException(
                        $"Unknown instruction '{Enum.GetName((Instruction)script[instructionPointer])}'");
#endif
            }
        }
    }
}