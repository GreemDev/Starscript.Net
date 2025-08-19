using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Starscript.Internal;
using Starscript.Util;

namespace Starscript;

public class MutableScript : ExecutableScript
{
    private readonly List<Value> _constants = [];

    public override ReadOnlySpan<Value> Constants => CollectionsMarshal.AsSpan(_constants);

    public ResizableBuffer<byte> CodeBuffer { get; } = new();

    public override ReadOnlySpan<byte> Code => CodeBuffer.RoSpan;

    public Script MoveToImmutable()
    {
        // Trim the excess (null) bytes before copying
        CodeBuffer.TrimExcess();

        var codeCopy = CodeBuffer.Span.ToArray();
        var constantsCopy = Constants.ToArray();

        // Clear the current script's memory, potentially useful for a reusable script instance system in the future(?)
        CodeBuffer.ResetAndClear();
        _constants.Clear();

#if DEBUG
        Compiler.DebugLog($"Resulting immutable script size in bytes after move: {codeCopy.Length * Unsafe.SizeOf<byte>()}");
#endif

        return new Script(
            codeCopy,
            constantsCopy
        );
    }

    #region Script writing

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

        foreach (var (idx, value) in _constants.Index())
        {
            if (value == constant)
            {
                constIndex = idx;
                break;
            }
        }

        if (constIndex is -1)
        {
            constIndex = _constants.Count;
            _constants.Add(constant);
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

    #endregion
    
    public override void Dispose()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(MutableScript), "Cannot dispose an already disposed Script.");
        
        _constants.Clear();
        _constants.TrimExcess();
        CodeBuffer.ResetAndClear();

#if DEBUG
        Compiler.DebugLog("Destroyed mutable script");
#endif

        IsDisposed = true;

        GC.SuppressFinalize(this);
    }
}