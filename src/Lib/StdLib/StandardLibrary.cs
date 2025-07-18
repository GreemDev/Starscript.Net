namespace Starscript;

public static partial class StandardLibrary
{
    private static readonly ThreadLocal<Random> tl_Random = new(() => new Random());

    public static string TimeFormat { get; set; } = "HH:mm:ss";
    public static string DateFormat { get; set; } = "dd/MM/yyyy";
    
    /// <summary>
    ///     Register all variables and functions present in the Starscript <see cref="StandardLibrary"/>.
    /// </summary>
    /// <param name="hv">The current <see cref="StarscriptHypervisor"/>.</param>
    /// <param name="unsafe">If this <see cref="StarscriptHypervisor"/> instance should have system information available. Not recommended in places where users can write & run Starscript but don't operate the machine it runs on.</param>
    /// <returns>The current <see cref="StarscriptHypervisor"/>, for chaining convenience.</returns>
    public static StarscriptHypervisor WithStandardLibrary(this StarscriptHypervisor hv, bool @unsafe = false)
    {
        hv = hv
            .WithStandardLibraryTime()
            .WithStandardLibraryMath()
            .WithStandardLibraryStrings();

        if (@unsafe)
            hv = hv.WithStandardLibraryUnsafe();

        return hv;
    }

    /// <summary>
    ///     Register all variables and functions present in the Starscript <see cref="StandardLibrary"/> time module.
    /// </summary>
    /// <param name="hv">The current <see cref="StarscriptHypervisor"/>.</param>
    public static StarscriptHypervisor WithStandardLibraryTime(this StarscriptHypervisor hv) => hv
        .Set("time", () => DateTime.Now.ToString(TimeFormat))
        .Set("date", () => DateTime.Now.ToString(DateFormat))
        .Set("timeUtc", () => DateTime.UtcNow.ToString(TimeFormat))
        .Set("dateUtc", () => DateTime.UtcNow.ToString(DateFormat));
}