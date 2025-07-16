using Starscript.Internal;

namespace Starscript;

public class Script
{
    private byte[] _code;
    
    public int Size { get; private set; }
    
    public List<Value> Constants { get; } = [];

    public ReadOnlySpan<byte> CodeBuffer => _code;

    public Script()
    {
        _code = new byte[8];
        Array.Fill(_code, (byte)0);
    }

    /// <summary>
    ///     Write an <see cref="Instruction"/> to this <see cref="Script"/>.
    /// </summary>
    public void Write(Instruction insn) => WriteByte((byte)insn);

    /// <summary>
    ///     Write an <see cref="Instruction"/> with an additional byte to this <see cref="Script"/>.
    /// </summary>
    public void Write(Instruction insn, byte b)
    {
        Write(insn);
        WriteByte(b);
    }

    /// <summary>
    ///     Write an <see cref="Instruction"/> with an additional constant to this <see cref="Script"/>.
    /// </summary>
    public void Write(Instruction insn, Value constant)
    {
        Write(insn);
        WriteConstant(constant);
    }

    /// <summary>
    ///     Write a constant <see cref="Value"/> to this <see cref="Script"/>.
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
        
        WriteByte((byte)constIndex);
    }

    /// <summary>
    ///     Begin a jump instruction.
    /// </summary>
    /// <returns>The location of the written <see cref="Instruction"/>.</returns>
    public int WriteJump(Instruction insn)
    {
        Write(insn);
        WriteByte(0);
        WriteByte(0);

        return Size - 2;
    }

    /// <summary>
    ///     End a jump instruction.
    /// </summary>
    public void PatchJump(int offset)
    {
        int jump = Size - offset - 2;

        _code[offset] = (byte)((jump >> 8) & 0xFF);
        _code[offset + 1] = (byte)(jump & 0xFF);
    }
    
    #region Buffer logic

    private void WriteByte(byte b)
    {
        GrowIfNeeded();
        _code[Size++] = b;
    }

    private void GrowIfNeeded()
    {
        if (Size >= _code.Length)
        {
            byte[] newBuffer = new byte[(int)(_code.Length * 1.5)];
            Array.Copy(_code, 0, newBuffer, 0, _code.Length);
            _code = newBuffer;
        }
    }

    #endregion
}