namespace Starscript;

public class Script
{
    private readonly byte[] _code;

    private readonly IReadOnlyList<Value> _constants;

    public ReadOnlySpan<byte> Code => _code;
    public IReadOnlyList<Value> Constants => _constants;
    
    public Script(byte[] codeBuffer, IReadOnlyList<Value> constants)
    {
        _code = codeBuffer;
        _constants = constants;
    }
    
    public byte GetByteAt(int idx) => _code[idx];
    
    public int GetMaskedByteAt(int idx) => GetByteAt(idx) & 0xFF;

    public StringSegment Execute(StarscriptHypervisor hypervisor) => hypervisor.Run(this);
}