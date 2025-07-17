using System.Text;

namespace Starscript;

public class StringSegment
{
    // ReSharper disable once InconsistentNaming
    private static readonly ThreadLocal<StringBuilder> tl_StringBuilder = new(() => new StringBuilder());
    
    public int Index { get; }
    public string Content { get; }

    public StringSegment(int index, string content)
    {
        Index = index;
        Content = content;
    }
    
    public StringSegment? Next { get; set; }

    public override string ToString()
    {
        var sb = tl_StringBuilder.Value!;
        sb.Length = 0;

        var segment = this;

        while (segment != null)
        {
            sb.Append(segment.Content);
            segment = segment.Next;
        }

        return sb.ToString();
    }
}