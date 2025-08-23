using System.Runtime.CompilerServices;
using System.Text;
using Starscript.Internal;

namespace Starscript;

public partial class StarscriptHypervisor
{
    protected override void Section(
        ref StringBuilder sb, 
        ref ExecutableScript script, 
        ref StringSegment firstSegment, 
        ref StringSegment segment, 
        ref int index, 
        ref int insnPtr)
    {
        if (Unsafe.IsNullRef(ref firstSegment))
        {
            firstSegment = new StringSegment(index, sb.ToString());
            segment = firstSegment;
        }
        else if (!Unsafe.IsNullRef(ref segment))
        {
            segment.Next = new StringSegment(index, sb.ToString());
            segment = segment.Next;
        }

        sb.Length = 0;
        index = script[insnPtr++];
    }
}