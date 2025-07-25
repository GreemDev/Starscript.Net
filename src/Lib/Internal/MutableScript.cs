﻿using System.Runtime.CompilerServices;
using Starscript.Internal;
using Starscript.Util;

namespace Starscript;

public class MutableScript
{
    public List<Value> Constants { get; } = [];

    public ResizableBuffer<byte> CodeBuffer { get; } = new();

    public Script MoveToImmutable()
    {
        // Trim the excess (null) bytes before copying
        CodeBuffer.TrimExcess();

        var codeCopy = CodeBuffer.Span.ToArray();
        var constantsCopy = new List<Value>(Constants);

        // Clear the current script's memory, potentially useful for a reusable script instance system in the future(?)
        CodeBuffer.ResetAndClear();
        Constants.Clear();

#if DEBUG
        Compiler.DebugLog($"Resulting immutable script size in bytes after move: {codeCopy.Length * Unsafe.SizeOf<byte>()}");
#endif

        return new Script(
            codeCopy,
            constantsCopy.AsReadOnly()
        );
    }

    /// <summary>
    ///     Write an <see cref="Instruction"/> to this <see cref="MutableScript"/>.
    /// </summary>
    public void Write(Instruction insn) => CodeBuffer.Write((byte)insn);

    /// <summary>
    ///     Write an <see cref="Instruction"/> with an additional byte to this <see cref="MutableScript"/>.
    /// </summary>
    public void Write(Instruction insn, byte b)
    {
        Write(insn);
        CodeBuffer.Write(b);
    }

    /// <summary>
    ///     Write an <see cref="Instruction"/> with an additional constant to this <see cref="MutableScript"/>.
    /// </summary>
    public void Write(Instruction insn, Value constant)
    {
        Write(insn);
        WriteConstant(constant);
    }

    /// <summary>
    ///     Write a constant <see cref="Value"/> to this <see cref="MutableScript"/>.
    /// </summary>
    public void WriteConstant(Value constant)
    {
        var constIndex = -1;

        foreach (var (idx, value) in Constants.Index())
        {
            if (value == constant)
            {
                constIndex = idx;
                break;
            }
        }

        if (constIndex is -1)
        {
            constIndex = Constants.Count;
            Constants.Add(constant);
        }
        
        CodeBuffer.Write((byte)constIndex);
    }

    /// <summary>
    ///     Begin a jump instruction.
    /// </summary>
    /// <returns>The location of the written <see cref="Instruction"/>.</returns>
    public int WriteJump(Instruction insn)
    {
        Write(insn);
        CodeBuffer.Write(0);
        CodeBuffer.Write(0);

        return CodeBuffer.CurrentSize - 2;
    }

    /// <summary>
    ///     End a jump instruction.
    /// </summary>
    public void PatchJump(int offset)
    {
        int jump = CodeBuffer.CurrentSize - offset - 2;

        CodeBuffer.Span[offset] = (byte)((jump >> 8) & 0xFF);
        CodeBuffer.Span[offset + 1] = (byte)(jump & 0xFF);
    }
}