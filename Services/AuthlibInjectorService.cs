using System.Security.Cryptography;
using System.Text.Json;

namespace Helio.Services;

/// <summary>
/// Downloads and caches authlib-injector, the Java agent that redirects
/// Minecraft's authentication/session/skin servers to Ely.by.
///
/// The JAR is stored next to the launcher data (~/.local/share/Helio/authlib-injector.jar)
/// and reused for every launch, so it is only downloaded once.
/// </summary>
public static class AuthlibInjectorService
{
    /// <summary>Value passed after '=' to the javaagent. Ely.by's official shortcut.</summary>
    public const string AgentArgument = "ely.by";

    private const string LatestJsonUrl =
        "https://authlib-injector.yushi.moe/artifact/latest.json";

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromMinutes(5) };

    public static string JarPath => Path.Combine(PathService.RootDir, "authlib-injector.jar");

    /// <summary>
    /// Ensures a valid authlib-injector JAR exists. Returns false when the
    /// download or checksum verification failed.
    /// </summary>
    public static async Task<bool> EnsureAsync()
    {
        if (IsUsableJar(JarPath)) return true;

        try
        {
            Directory.CreateDirectory(PathService.RootDir);

            using var metaResp = await Http.GetAsync(LatestJsonUrl);
            metaResp.EnsureSuccessStatusCode();

            using var meta = JsonDocument.Parse(
                await metaResp.Content.ReadAsStringAsync());

            if (!meta.RootElement.TryGetProperty("download_url", out var urlEl))
                return false;

            var url = urlEl.GetString();
            if (string.IsNullOrWhiteSpace(url)) return false;

            string? expected = null;
            if (meta.RootElement.TryGetProperty("checksums", out var sums) &&
                sums.TryGetProperty("sha256", out var shaEl))
                expected = shaEl.GetString();

            var bytes = await Http.GetByteArrayAsync(url);

            if (!string.IsNullOrWhiteSpace(expected))
            {
                var actual = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
                if (!actual.Equals(expected.Trim(), StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            var tmp = JarPath + ".tmp";
            await File.WriteAllBytesAsync(tmp, bytes);
            File.Move(tmp, JarPath, overwrite: true);

            return IsUsableJar(JarPath);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsUsableJar(string path) =>
        File.Exists(path) && new FileInfo(path).Length > 100_000;
}
