using System.Net.Http.Headers;

namespace Starscript;

public static partial class StandardLibrary
{
    /// <summary>
    ///     Register all variables and functions present in the Starscript <see cref="StandardLibrary"/> HTTP module.
    /// </summary>
    /// <param name="hv">The current <see cref="StarscriptHypervisor"/>.</param>
    /// <returns>The current <see cref="StarscriptHypervisor"/>, for chaining convenience.</returns>
    public static StarscriptHypervisor WithStandardLibraryHttp(this StarscriptHypervisor hv) => hv
        .NewSubMap("http", http => http
            .Set("get", HttpGet));

    public static Value HttpGet(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(1));

        var url = ctx.NextString(1);

        var http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(2),
            DefaultRequestHeaders =
            {
                UserAgent = { new ProductInfoHeaderValue("StarscriptHttp", "1.0.0") }
            }
        };

        try
        {
            return http.GetStringAsync(url).Result;
        }
        catch (InvalidOperationException)
        {
            throw ctx.Error("'{0}' is not an absolute URI", url);
        }
        catch (HttpRequestException hre)
        {
            throw ctx.Error("GET request to {0} failed (1): {2}", url, hre.StatusCode, hre.Message);
        }
        catch (TaskCanceledException)
        {
            throw ctx.Error("GET request to {0} timed out", url);
        }
        catch (UriFormatException ufe)
        {
            throw ctx.Error("Malformed input URL '{0}': {1}", url, ufe.Message);
        }
        finally
        {
            http.Dispose();
        }
    }
}