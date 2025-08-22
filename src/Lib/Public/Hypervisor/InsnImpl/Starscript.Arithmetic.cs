namespace Starscript;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class StarscriptHypervisor
{
    protected virtual void Add()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() + b.GetNumber());
        else if (a.IsString)
            Push(a.GetString() + b);
        else
            throw Error("Can only add 2 numbers, or 1 string and any other value.");
    }

    protected virtual void Negate()
    {
        var a = Pop();

        if (!a.IsNumber)
            throw Error("Negation requires a number.");

        Push(-a.GetNumber());
    }

    protected virtual void Subtract()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() - b.GetNumber());
        else
            throw Error("Can only subtract 2 numbers.");
    }

    protected virtual void Multiply()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() * b.GetNumber());
        else
            throw Error("Can only multiply 2 numbers.");
    }

    protected virtual void Divide()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() / b.GetNumber());
        else
            throw Error("Can only divide 2 numbers.");
    }

    protected virtual void Modulo()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(a.GetNumber() % b.GetNumber());
        else
            throw Error("Can only modulo 2 numbers.");
    }

    protected virtual void Power()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(Math.Pow(a.GetNumber(), b.GetNumber()));
        else
            throw Error("Can only power 2 numbers.");
    }

    protected virtual void RightShift()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(Convert.ToInt32(a.GetNumber()) >> Convert.ToInt32(double.Truncate(b.GetNumber())));
        else
            throw Error(">> operation requires 2 numbers.");
    }

    protected virtual void LeftShift()
    {
        var (a, b) = PopPair();

        if (a.IsNumber && b.IsNumber)
            Push(Convert.ToInt32(a.GetNumber()) << Convert.ToInt32(double.Truncate(b.GetNumber())));
        else
            throw Error("<< operation requires 2 numbers.");
    }
}