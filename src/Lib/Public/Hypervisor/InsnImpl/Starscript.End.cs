using System.Runtime.CompilerServices;
using System.Text;

namespace Starscript;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class StarscriptHypervisor
{
    protected virtual StringSegment EndExecution(
        ref StringBuilder sb, 
        ref StringSegment firstSegment, 
        ref StringSegment segment, 
        int index)
    {
        if (!_persistentLocals)
            ClearLocals();

        if (!Unsafe.IsNullRef(ref firstSegment) && !Unsafe.IsNullRef(ref segment))
        {
            segment.Next = new StringSegment(index, sb.ToString());
            return firstSegment;
        }

        return new StringSegment(index, sb.ToString());
    }
}