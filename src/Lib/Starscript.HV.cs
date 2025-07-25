﻿using System.Text;
using Starscript.Internal;
using static Starscript.Internal.Instruction;

namespace Starscript;

public partial class StarscriptHypervisor
{
    internal StringSegment RunImpl(Script script, StringBuilder sb)
    {
        _stack.Clear();

        sb.Length = 0;

        int instructionPointer = 0;

        StringSegment? firstSegment = null;
        StringSegment? segment = null;
        int index = 0;

        while (true)
        {
#if DEBUG
            var idx = instructionPointer++;
            var insn = (Instruction)script.GetByteAt(idx);
            DebugLog($"Processing {Enum.GetName(insn)} instruction @ ip 0x{idx:x8} ({idx})");

            switch (insn)
#else
            switch ((Instruction)script.GetByteAt(instructionPointer++))
#endif
            {
                case Constant:
                {
                    Push(script.Constants[script.GetMaskedByteAt(instructionPointer++)]);
                    break;
                }
                case Null: Push(Value.Null); break;
                case True: Push(true); break;
                case False: Push(false); break;
                case Instruction.Pop: Pop(); break;

                case Add:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() + b.GetNumber());
                    else if (a.IsString)
                        Push(a.GetString() + b);
                    else
                        throw Error("Can only add 2 numbers, or 1 string and any other value.");

                    break;
                }
                case Subtract:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() - b.GetNumber());
                    else
                        throw Error("Can only subtract 2 numbers.");

                    break;
                }
                case Multiply:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() * b.GetNumber());
                    else
                        throw Error("Can only multiply 2 numbers.");

                    break;
                }
                case Divide:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() / b.GetNumber());
                    else
                        throw Error("Can only divide 2 numbers.");

                    break;
                }
                case Modulo:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() % b.GetNumber());
                    else
                        throw Error("Can only modulo 2 numbers.");

                    break;
                }
                case Power:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(Math.Pow(a.GetNumber(), b.GetNumber()));
                    else
                        throw Error("Can only power 2 numbers.");

                    break;
                }

                case AddConstant:
                {
                    var b = script.Constants[script.GetMaskedByteAt(instructionPointer++)];
                    var a = Pop();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() + b.GetNumber());
                    else if (a.IsString)
                        Push(a.GetString() + b);
                    else
                        throw Error("Can only add 2 numbers, or 1 string and any other value.");

                    break;
                }

                case Not:
                {
                    Push(!Pop().IsTruthy);
                    break;
                }
                case Negate:
                {
                    var a = Pop();

                    if (!a.IsNumber)
                        throw Error("Negation requires a number.");

                    Push(-a.GetNumber());

                    break;
                }

                case Instruction.Equals:
                {
                    // ReSharper disable once EqualExpressionComparison
                    Push(Pop().Equals(Pop()));
                    break;
                }
                case NotEquals:
                {
                    // ReSharper disable once EqualExpressionComparison
                    Push(!Pop().Equals(Pop()));
                    break;
                }
                case Greater:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() > b.GetNumber());
                    else
                        throw Error("> operation requires 2 numbers.");

                    break;
                }
                case GreaterEqual:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() >= b.GetNumber());
                    else
                        throw Error(">= operation requires 2 numbers.");

                    break;
                }
                case Less:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() < b.GetNumber());
                    else
                        throw Error("< operation requires 2 numbers.");

                    break;
                }
                case LessEqual:
                {
                    var (a, b) = PopPair();

                    if (a.IsNumber && b.IsNumber)
                        Push(a.GetNumber() <= b.GetNumber());
                    else
                        throw Error("<= operation requires 2 numbers.");

                    break;
                }

                case Variable:
                {
                    var name = script.Constants[script.GetMaskedByteAt(instructionPointer++)].GetString();

                    Push((Locals?.GetRaw(name) ?? Globals.GetRaw(name))?.Invoke());

                    break;
                }
                case Get:
                {
                    var name = script.Constants[script.GetMaskedByteAt(instructionPointer++)].GetString();

                    var value = Pop();

                    Push(!value.IsMap
                        ? Value.Null
                        : value.GetMap().GetRaw(name)?.Invoke()
                    );

                    break;
                }
                case Call:
                {
                    var argCount = script.GetByteAt(instructionPointer++);

                    var a = Peek(argCount);

                    if (a.IsNull)
                        throw Error("Unknown function.");
                    
                    if (!a.IsFunction)
                        throw Error("Tried to call {0}, can only call functions.", Enum.GetName(a.Type));

                    var result = a.GetFunction()(this, argCount);
                    Pop();
                    Push(result);

                    break;
                }

                case Jump:
                {
                    var jump = (script.GetMaskedByteAt(instructionPointer++) << 8)
                               | script.GetMaskedByteAt(instructionPointer++);

                    instructionPointer += jump;

                    break;
                }
                case JumpIfTrue:
                {
                    var jump = (script.GetMaskedByteAt(instructionPointer++) << 8)
                               | script.GetMaskedByteAt(instructionPointer++);

                    if (Peek().IsTruthy)
                        instructionPointer += jump;

                    break;
                }
                case JumpIfFalse:
                {
                    var jump = (script.GetMaskedByteAt(instructionPointer++) << 8)
                               | script.GetMaskedByteAt(instructionPointer++);

                    if (!Peek().IsTruthy)
                        instructionPointer += jump;

                    break;
                }

                case Section:
                {
                    if (firstSegment is null)
                    {
                        firstSegment = new StringSegment(index, sb.ToString());
                        segment = firstSegment;
                    }
                    else
                    {
                        segment!.Next = new StringSegment(index, sb.ToString());
                        segment = segment.Next;
                    }

                    sb.Length = 0;
                    index = script.GetByteAt(instructionPointer++);
                    break;
                }

                case Instruction.Append:
                {
                    Append(sb, Pop());
                    break;
                }
                case ConstantAppend:
                {
                    Append(sb, script.Constants[script.GetMaskedByteAt(instructionPointer++)]);
                    break;
                }
                case VariableAppend:
                {
                    var name = script.Constants[script.GetMaskedByteAt(instructionPointer++)].GetString();

                    Append(sb, (Locals?.GetRaw(name) ?? Globals.GetRaw(name))?.Invoke());

                    break;
                }
                case GetAppend:
                {
                    var name = script.Constants[script.GetMaskedByteAt(instructionPointer++)].GetString();

                    var value = Pop();

                    Append(sb, value.IsMap
                        ? value.GetMap().GetRaw(name)?.Invoke()
                        : null);

                    break;
                }
                case CallAppend:
                {
                    var argCount = script.GetByteAt(instructionPointer++);

                    var a = Peek(argCount);
                    
                    if (a.IsNull)
                        throw Error("Unknown function.");

                    if (!a.IsFunction)
                        throw Error("Tried to call {0}, can only call functions.", Enum.GetName(a.Type));

                    var result = a.GetFunction()(this, argCount);
                    Pop();
                    Append(sb, result);

                    break;
                }

                case VariableGet:
                {
                    Value value;

                    {
                        // Variable
                        var name = script.Constants[script.GetMaskedByteAt(instructionPointer++)].GetString();
                        value = (Locals?.GetRaw(name) ?? Globals.GetRaw(name))?.Invoke() ?? Value.Null;
                    }

                    {
                        // Get
                        var name = script.Constants[script.GetMaskedByteAt(instructionPointer++)].GetString();

                        if (!value.IsMap)
                        {
                            Push(null);
                            break;
                        }

                        Push(value.GetMap().GetRaw(name)?.Invoke());
                    }

                    break;
                }
                case VariableGetAppend:
                {
                    Value? value;

                    {
                        // Variable
                        var name = script.Constants[script.GetMaskedByteAt(instructionPointer++)].GetString();
                        value = (Locals?.GetRaw(name) ?? Globals.GetRaw(name))?.Invoke() ?? Value.Null;
                    }

                    {
                        // Get
                        var name = script.Constants[script.GetMaskedByteAt(instructionPointer++)].GetString();

                        if (!value.IsMap)
                        {
                            Push(null);
                            break;
                        }

                        value = value.GetMap().GetRaw(name)?.Invoke();
                    }

                    Append(sb, value);

                    break;
                }
                case End:
#if DEBUG
                    DebugLog($"Encountered {Enum.GetName(End)} instruction. Breaking execution.");
#endif

                    goto EndExecution;
                default:
                    throw new InvalidOperationException(
                        $"Unknown instruction '{Enum.GetName((Instruction)script.GetByteAt(instructionPointer))}'");
            }
        }

        EndExecution:

        if (!_persistentLocals) 
            Locals = null;

        if (firstSegment != null)
        {
            segment!.Next = new StringSegment(index, sb.ToString());
            return firstSegment;
        }

        return new StringSegment(index, sb.ToString());
    }

    private static void Append(StringBuilder sb, Value? value) => sb.Append(value ?? Value.Null);
}