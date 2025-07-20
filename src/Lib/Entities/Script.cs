namespace Starscript;

public class Script
{
    private readonly byte[] _code;

    public ReadOnlySpan<byte> Code => _code;
    
    public IReadOnlyList<Value> Constants { get; }

    public Script(byte[] codeBuffer, IReadOnlyList<Value> constants)
    {
        _code = codeBuffer;
        Constants = constants;
    }
    
    public byte GetByteAt(int idx) => _code[idx];
    
    public int GetMaskedByteAt(int idx) => GetByteAt(idx) & 0xFF;

    public StringSegment Execute(StarscriptHypervisor hypervisor) => hypervisor.Run(this);
}