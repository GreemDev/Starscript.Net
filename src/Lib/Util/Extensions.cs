using System.Text;

namespace Starscript.Util;

public static class Extensions
{
    public static string Pluralize(this string word, int quantity, Plurality? plurality = null,
        bool prefixQuantity = false)
    {
        plurality ??= Plurality.Simple;

        var sb = new StringBuilder();

        if (prefixQuantity)
            sb.Append($"{quantity} ");

        sb.Append(quantity is 1
            ? word
            : plurality.Format(word)
        );

        return sb.ToString();
    }
}

public abstract record Plurality(string Ending, uint TrimAmount = 0)
{
    public static readonly Plurality Simple = new SimplePlurality();
    public static readonly Plurality Es = new EsPlurality();
    public static readonly Plurality Ies = new IesPlurality();

    private sealed record SimplePlurality() : Plurality("s");

    private sealed record EsPlurality() : Plurality("es");

    private sealed record IesPlurality() : Plurality("ies", 1);


    public string Format(string word)
        => (TrimAmount > 0
                ? string.Join(string.Empty, word.SkipLast((int)TrimAmount))
                : word
            ) + Ending;
}