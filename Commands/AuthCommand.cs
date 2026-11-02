using Helio;
using Helio.Services;
using Spectre.Console;

namespace Helio.Commands;

public class AuthCommand
{
    private readonly AuthService _auth;

    public AuthCommand(AuthService auth) => _auth = auth;

    public async Task ExecuteAsync(string[] args)
    {
        var sub = args.Length > 0 ? args[0].ToLowerInvariant() : "status";

        switch (sub)
        {
            // ── Login ──────────────────────────────────────────────────────────
            case "login" or "in":
            {
                // "auth login elyby [alias]" and "auth elyby [alias]" are equivalent.
                var rest = args.Length > 1 ? args[1..] : Array.Empty<string>();
                if (rest.Length > 0 && IsElyByKeyword(rest[0]))
                {
                    var (usePassword, elyAlias) = ParseElyArgs(rest[1..]);
                    if (usePassword)
                        await _auth.LoginElyByPasswordAsync(elyAlias);
                    else
                        await _auth.LoginElyByAsync(elyAlias);
                }
                else
                {
                    // Optional alias: "auth login <alias>" saves the account under that name
                    var alias = rest.Length > 0 ? string.Join(' ', rest) : null;
                    await _auth.LoginAsync(alias);
                }
                break;
            }

            // ── Ely.by login (OAuth device code; --password for fallback) ──────
            case "elyby" or "ely" or "ely.by":
            {
                var (usePassword, alias) = ParseElyArgs(
                    args.Length > 1 ? args[1..] : Array.Empty<string>());

                if (usePassword)
                    await _auth.LoginElyByPasswordAsync(alias);
                else
                    await _auth.LoginElyByAsync(alias);
                break;
            }

            // ── Logout (active account) ────────────────────────────────────────
            case "logout" or "out":
                _auth.Logout();
                break;

            // ── Status / whoami ────────────────────────────────────────────────
            case "status" or "s" or "whoami":
                _auth.PrintStatus();
                break;

            // ── List all saved accounts ────────────────────────────────────────
            case "accounts" or "list" or "ls":
                _auth.ListAccounts();
                break;

            // ── Switch active account ──────────────────────────────────────────
            case "switch" or "use":
                if (args.Length < 2)
                    AnsiConsole.MarkupLine(
                        $"[{UiTheme.AccentMarkup}]Usage:[/] auth switch [grey]<alias>[/]");
                else
                    _auth.SwitchAccount(string.Join(' ', args[1..]));
                break;

            // ── Remove a saved account ─────────────────────────────────────────
            case "remove" or "rm" or "delete":
                if (args.Length < 2)
                    AnsiConsole.MarkupLine(
                        $"[{UiTheme.AccentMarkup}]Usage:[/] auth remove [grey]<alias>[/]");
                else
                    _auth.RemoveAccount(string.Join(' ', args[1..]));
                break;

            default:
                AnsiConsole.MarkupLine(
                    $"[{UiTheme.AccentMarkup}]Unknown auth subcommand:[/] {Markup.Escape(sub)}");
                AnsiConsole.MarkupLine(
                    "[dim]Usage: auth login [alias] | auth elyby [--password] [alias] | " +
                    "auth logout | auth status | auth accounts | auth switch <alias> | " +
                    "auth remove <alias>[/]");
                break;
        }
    }

    private static bool IsElyByKeyword(string value) =>
        value.Equals("elyby", StringComparison.OrdinalIgnoreCase) ||
        value.Equals("ely", StringComparison.OrdinalIgnoreCase) ||
        value.Equals("ely.by", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Splits Ely.by arguments into an optional --password flag and the
    /// remaining words (the account alias).
    /// </summary>
    private static (bool UsePassword, string? Alias) ParseElyArgs(string[] args)
    {
        var usePassword = false;
        var aliasParts = new List<string>();

        foreach (var arg in args)
        {
            if (arg is "--password" or "-p" or "--yggdrasil")
                usePassword = true;
            else
                aliasParts.Add(arg);
        }

        return (usePassword, aliasParts.Count > 0 ? string.Join(' ', aliasParts) : null);
    }
}
