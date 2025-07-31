using System.Runtime.InteropServices;

namespace Starscript;

public static partial class StandardLibrary
{
    /// <summary>
    ///     Register all variables and functions present in the Starscript <see cref="StandardLibrary"/> unsafe module.
    /// </summary>
    /// <param name="hv">The current <see cref="StarscriptHypervisor"/>.</param>
    /// <returns>The current <see cref="StarscriptHypervisor"/>, for chaining convenience.</returns>
    public static StarscriptHypervisor WithStandardLibraryUnsafe(this StarscriptHypervisor hv) => hv
        .NewSubMap("env", map => map
            .SetToString(() => Environment.MachineName)
            .Set("coreCount", () => Environment.ProcessorCount)
            .Set("pid", () => Environment.ProcessId)
            .Set("processPath", () => Environment.ProcessPath)
            .Set("dotnet", () => RuntimeInformation.FrameworkDescription)
            .Set("runtimeId", () => RuntimeInformation.RuntimeIdentifier)
            .NewSubMap("os", osMap =>
            {
                osMap.SetToString(() => RuntimeInformation.OSDescription);
                osMap.Set("isBrowser", OperatingSystem.IsBrowser());
                osMap.Set("isWasi", OperatingSystem.IsWasi());
                osMap.Set("isWindows", OperatingSystem.IsWindows());
                osMap.Set("isMac", OperatingSystem.IsMacOS());
                osMap.Set("isMacCatalyst", OperatingSystem.IsMacCatalyst());
                osMap.Set("isIOS", OperatingSystem.IsIOS());
                osMap.Set("isTvOS", OperatingSystem.IsTvOS());
                osMap.Set("isAndroid", OperatingSystem.IsAndroid());
                osMap.Set("isLinux", OperatingSystem.IsLinux());
                osMap.Set("isFreeBsd", OperatingSystem.IsFreeBSD());
            }));
}