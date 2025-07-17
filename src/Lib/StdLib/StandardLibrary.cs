namespace Starscript;

public static partial class StandardLibrary
{
    private static readonly ThreadLocal<Random> tl_Random = new(() => new Random());

    public static string TimeFormat { get; set; } = "HH:mm:ss";
    public static string DateFormat { get; set; } = "dd/MM/yyyy";

    public static StarscriptHypervisor WithStandardLibrary(this StarscriptHypervisor hv, bool withEnv = false)
    {
        hv = hv.WithStandardLibraryTime().WithStandardLibraryMath();

        if (withEnv)
            hv = hv.WithStandardLibraryEnv();

        return hv;
    }

    public static StarscriptHypervisor WithStandardLibraryTime(this StarscriptHypervisor hv) => hv
        .Set("time", () => DateTime.Now.ToString(TimeFormat))
        .Set("date", () => DateTime.Now.ToString(DateFormat))
        .Set("timeUtc", () => DateTime.UtcNow.ToString(TimeFormat))
        .Set("dateUtc", () => DateTime.UtcNow.ToString(DateFormat));
}