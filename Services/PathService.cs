namespace Helio.Services;

public static class PathService
{
    private static readonly HashSet<char> InvalidChars = new(Path.GetInvalidFileNameChars());

    public static readonly string RootDir = GetRootDir();

    private static string GetRootDir()
    {
        var root = RootDirFor("Helio");
        MigrateLegacyData(root);
        return root;
    }

    private static string RootDirFor(string appName)
    {
        if (PlatformHelper.IsWindows)
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);

        var home = Environment.GetEnvironmentVariable("HOME") ?? "";

        // macOS convention: ~/Library/Application Support/<app>
        if (PlatformHelper.IsMacOS && !string.IsNullOrEmpty(home))
            return Path.Combine(home, "Library", "Application Support", appName);

        // Linux: respect XDG_DATA_HOME, otherwise ~/.local/share/<app>
        var xdg = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
        if (!string.IsNullOrEmpty(xdg))
            return Path.Combine(xdg, appName);
        if (!string.IsNullOrEmpty(home))
            return Path.Combine(home, ".local", "share", appName);

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
    }

    /// <summary>
    /// One-time migration from the original McSH data directory. Moving (rather
    /// than copying) keeps instances, worlds, settings, and accounts intact and
    /// avoids duplicating multi-gigabyte asset caches.
    /// </summary>
    private static void MigrateLegacyData(string newRoot)
    {
        try
        {
            if (Directory.Exists(newRoot)) return;

            var legacy = RootDirFor("McSH");
            if (string.Equals(legacy, newRoot, StringComparison.OrdinalIgnoreCase)) return;
            if (!Directory.Exists(legacy)) return;

            Directory.Move(legacy, newRoot);
        }
        catch
        {
            // If migration fails, Helio simply starts with a fresh data directory.
        }
    }

    // ── Instance paths ────────────────────────────────────────────────────────

    public static string InstancesDir                    => Path.Combine(RootDir, "instances");
    public static string InstanceDir(string name)        => Path.Combine(InstancesDir, Sanitize(name));
    public static string InstanceManifest(string name)   => Path.Combine(InstanceDir(name), "instance.json");
    public static string LaunchCachePath(string name)    => Path.Combine(InstanceDir(name), "launch_cache.json");
    public static string ModsDir(string name)            => Path.Combine(InstanceDir(name), "mods");
    public static string ModsJsonPath(string name)       => Path.Combine(InstanceDir(name), "mods.json");
    public static string NativesDir(string name)         => Path.Combine(InstanceDir(name), "natives");

    // ── Content paths ─────────────────────────────────────────────────────────

    public static string ResourcePacksDir(string name) => Path.Combine(InstanceDir(name), "resourcepacks");
    public static string ShaderPacksDir(string name)   => Path.Combine(InstanceDir(name), "shaderpacks");
    public static string PluginsDir(string name)        => Path.Combine(InstanceDir(name), "plugins");
    public static string DatapacksDir(string name)      => Path.Combine(InstanceDir(name), "datapacks");

    // ── Shared version cache ──────────────────────────────────────────────────

    public static string VersionsDir                     => Path.Combine(RootDir, "versions");
    public static string VersionDir(string version)      => Path.Combine(VersionsDir, version);
    public static string ClientJarPath(string version)   => Path.Combine(VersionDir(version), $"{version}.jar");
    public static string VersionJsonPath(string version) => Path.Combine(VersionDir(version), $"{version}.json");

    public static string LoaderProfilePath(string loaderName, string mcVersion, string loaderVersion) =>
        Path.Combine(VersionsDir, $"{loaderName}-{Sanitize(mcVersion)}-{Sanitize(loaderVersion)}.json");

    // ── Shared libraries ──────────────────────────────────────────────────────

    public static string LibrariesDir => Path.Combine(RootDir, "libraries");

    // ── Shared assets ─────────────────────────────────────────────────────────

    public static string AssetsDir      => Path.Combine(RootDir, "assets");
    public static string AssetIndexDir  => Path.Combine(AssetsDir, "indexes");
    public static string AssetObjectDir => Path.Combine(AssetsDir, "objects");

    public static string AssetIndexPath(string indexId) =>
        Path.Combine(AssetIndexDir, $"{indexId}.json");

    public static string AssetObjectPath(string hash) =>
        Path.Combine(AssetObjectDir, hash[..2], hash);

    // ── Exports ───────────────────────────────────────────────────────────────

    public static string ExportsDir => Path.Combine(RootDir, "exports");

    // ── Auth ──────────────────────────────────────────────────────────────────

    /// <summary>Legacy single-account token file. Kept for migration only.</summary>
    public static string AuthPath => Path.Combine(RootDir, "auth.json");

    /// <summary>Multi-account store introduced in v0.4.0.</summary>
    public static string AccountsPath => Path.Combine(RootDir, "accounts.json");

    // ── Managed JDKs (downloaded by Helio) ────────────────────────────────────

    public static string ManagedJdksDir              => Path.Combine(RootDir, "jdks");
    public static string ManagedJdkDir(int major)    => Path.Combine(ManagedJdksDir, major.ToString());

    // ── Java version cache ────────────────────────────────────────────────────

    public static string JavaCachePath => Path.Combine(RootDir, "java_cache.json");

    // ── Settings ──────────────────────────────────────────────────────────────

    public static string SettingsPath => Path.Combine(RootDir, "settings.json");

    // ── Session state (persists active instance across REPL sessions) ─────────

    public static string SessionPath => Path.Combine(RootDir, "session.json");

    // ── Backups ───────────────────────────────────────────────────────────────

    public static string BackupsDir                   => Path.Combine(RootDir, "backups");
    public static string InstanceBackupsDir(string n) => Path.Combine(BackupsDir, n);

    // ── Mod profiles ──────────────────────────────────────────────────────────

    public static string ModProfilesPath(string instanceName) =>
        Path.Combine(InstanceDir(instanceName), "profiles.json");

    // ── Skins ─────────────────────────────────────────────────────────────────

    public static string SkinsDir          => Path.Combine(RootDir, "skins");
    public static string SkinsManifestPath => Path.Combine(SkinsDir, "skins.json");

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string Sanitize(string name) =>
        string.Concat(name.Select(c => InvalidChars.Contains(c) ? '_' : c));
}

