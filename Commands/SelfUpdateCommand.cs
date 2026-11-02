using Spectre.Console;

namespace Helio.Commands;

/// <summary>
/// HelioMC is a local fork and does not ship an auto-updater. The command is
/// kept for compatibility with the original command set.
/// </summary>
public class SelfUpdateCommand
{
    public static readonly string CurrentVersion =
        typeof(SelfUpdateCommand).Assembly.GetName().Version is { } v
            ? $"{v.Major}.{v.Minor}.{v.Build}"
            : "0.0.0";

    public Task ExecuteAsync()
    {
        AnsiConsole.MarkupLine("[yellow]Self-update is not available in HelioMC.[/]");
        AnsiConsole.MarkupLine(
            "[dim]HelioMC is a local fork — rebuild it from source to update.[/]");
        return Task.CompletedTask;
    }
}
