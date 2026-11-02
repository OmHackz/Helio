using System.Text.Json;
using Helio;
using Helio.Models;
using Spectre.Console;

namespace Helio.Services;

public static class SettingsService
{
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

    public static AppSettings Current { get; private set; } = new();

    public static void Load()
    {
        bool saveNeeded = false;

        try
        {
            var path = PathService.SettingsPath;
            if (!File.Exists(path))
            {
                Current = new AppSettings();
                saveNeeded = true;
            }
            else
            {
                var json = File.ReadAllText(path);
                Current = JsonSerializer.Deserialize<AppSettings>(json, Opts) ?? new AppSettings();
            }
        }
        catch (Exception ex)
        {
            Current = new AppSettings();
            AnsiConsole.MarkupLine($"[yellow]Warning:[/] failed to load settings, using defaults. {Markup.Escape(ex.Message)}");
        }

        // One-time rebrand migration: McSH settings defaulted to the crimson
        // theme, so switch those installs to Helio's orange accent.
        if (!Current.HelioMigrated)
        {
            Current.HelioMigrated = true;
            if (string.Equals(Current.Theme, "crimson", StringComparison.OrdinalIgnoreCase))
                Current.Theme = "helio";

            saveNeeded = true;
        }

        if (saveNeeded) Save();

        UiTheme.Apply(Current.Theme);
        LanguageService.Load(Current.Language);
    }

    public static void Save()
    {
        Directory.CreateDirectory(PathService.RootDir);
        File.WriteAllText(PathService.SettingsPath, JsonSerializer.Serialize(Current, Opts));
    }

    public static void ResetToDefaults()
    {
        Current = new AppSettings();
        Save();
        UiTheme.Apply(Current.Theme);
    }
}


