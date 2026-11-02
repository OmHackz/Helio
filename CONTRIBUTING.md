# Contributing to HelioMC

Thanks for considering a contribution. HelioMC is a small, terminal-first
project — issues, docs, translations, and focused pull requests are all valuable.

By participating, you agree to follow the [Code of Conduct](CODE_OF_CONDUCT.md).

## Ways to contribute

- **Report bugs** with the bug report template. Include `helio version`, your
  OS, and the exact commands you ran.
- **Request features** with the feature request template. Explain the problem
  before the solution.
- **Improve docs** — README, BUILD.md, command help, or error messages.
- **Translate** — English and Spanish live in `Languages/en.json` and
  `Languages/es.json`.
- **Test** — try a build on a platform or loader you use and report what breaks.
- **Code** — bug fixes, new commands, Modrinth integrations, performance work.

## Before you start

1. **Search first.** Check existing issues, pull requests, and discussions.
2. **Open an issue for anything non-trivial.** A quick issue avoids wasted work
   on a design that does not fit the project.
3. **Keep changes focused.** One logical change per pull request. Do not mix
   refactors, formatting, and features.
4. **Never commit secrets.** No tokens, passwords, `accounts.json`, or personal
   paths. See [SECURITY.md](SECURITY.md).

## Development setup

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git
- A terminal (HelioMC is a TUI; Windows Terminal works well)

### Build and run

```bash
git clone https://github.com/OmHackz/Helio.git
cd REPO

dotnet restore Helio.csproj
dotnet build Helio.csproj -c Release
dotnet bin/Release/net8.0/helio.dll
```

Run a single command without the REPL:

```bash
dotnet bin/Release/net8.0/helio.dll version
dotnet bin/Release/net8.0/helio.dll help
```

Self-contained publish:

```bash
dotnet publish Helio.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o out
```

Replace `linux-x64` with `linux-arm64`, `osx-arm64`, `osx-x64`, or `win-x64`.

### Tests

There is no automated test suite yet. Before opening a pull request, at minimum:

```bash
dotnet build Helio.csproj -c Release
python3 -m json.tool Languages/en.json > /dev/null
python3 -m json.tool Languages/es.json > /dev/null
dotnet bin/Release/net8.0/helio.dll version
dotnet bin/Release/net8.0/helio.dll help
dotnet bin/Release/net8.0/helio.dll ref
```

Then manually exercise the command you changed, including the error path. If
your change touches launching, test at least one real instance on a supported
loader. If it touches auth, test both Ely.by OAuth and the password fallback
where possible.

The GitHub `Build` workflow also validates JSON resources and runs a smoke test.

## Project layout

| Path | Purpose |
|---|---|
| `Program.cs` | Entry point, REPL loop, prompt, tab completion |
| `Commands/CommandRouter.cs` | Top-level verb dispatch and aliases |
| `Commands/*.cs` | One class per command group |
| `Services/AuthService.cs` | Ely.by OAuth, refresh, password fallback, accounts |
| `Services/AuthlibInjectorService.cs` | Downloads and verifies authlib-injector |
| `Services/GameLaunchService.cs` | Builds and starts the Minecraft process |
| `Services/InstanceStore.cs` | Instance discovery and manifests |
| `Services/ModrinthService.cs` | Modrinth API client |
| `Services/JavaService.cs` | JDK detection and download |
| `Services/PathService.cs` | Data directory and file paths |
| `Services/SettingsService.cs` | Settings load/save and migrations |
| `Services/LanguageService.cs` | Translation lookup |
| `State/AppState.cs` | Active instance and session persistence |
| `Models/` | Settings, accounts, instances, caches |
| `Languages/` | `en.json`, `es.json` |
| `UiTheme.cs` | Theme definitions and accent colours |

## Coding guidelines

- **C# style** follows the surrounding code: file-scoped namespaces, four-space
  indentation, `var` when the type is obvious, and expression-bodied members for
  small helpers.
- **Async** — use `async`/`await` and `Task` for I/O. Do not block on `.Result`.
- **Errors** — catch narrowly. User-facing failures should print a clear,
  escaped message rather than a stack trace. Use `Markup.Escape` for dynamic
  values in Spectre markup.
- **Spectre markup** — close every `[tag]`. Interpolated markup like
  `"[dim]... {value}[/]"` breaks when `value` contains brackets; escape it.
- **No new dependencies** without discussing the trade-off in an issue. The
  project deliberately stays small.
- **Platforms** — guard OS-specific behavior with `PlatformHelper`; do not
  assume Linux.
- **Compatibility** — data migrations must be one-time and backward compatible.
  Never delete user data without a backup or explicit confirmation.
- **Security** — do not log tokens, passwords, or full account files. Keep
  token refresh paths provider-aware.

### Adding a command

1. Add the verb to the dispatch switch in `Commands/CommandRouter.cs`.
2. Implement it in the appropriate `Commands/*.cs` group.
3. Add a usage line and a `helio ref` entry in `Commands/HelpCommand.cs`.
4. Add help strings to both `Languages/en.json` and `Languages/es.json`.
5. Add tab completions in `Program.cs` (`GetCompletions`).
6. Update `README.md` if the command is user-facing.
7. Test the command, its aliases, and its failure modes.

### Translations

- Add the key to **both** language files. `LanguageService` falls back to the
  key itself, which makes missing entries obvious.
- Keep placeholders and Spectre markup intact.
- Match the tone: direct, short, no exclamation marks.
- Validate the JSON before committing:

```bash
python3 -m json.tool Languages/en.json > /dev/null
python3 -m json.tool Languages/es.json > /dev/null
```

### Themes

Themes are defined in `UiTheme.cs` and the banner gradient in
`Services/BannerService.cs`. A theme needs:

- a unique key in `UiTheme.Themes`
- accent colours that render acceptably on dark and light terminals
- a banner gradient entry (or a sensible default)

The default is `helio` (orange). Do not change the default without discussion.

## Commit conventions

HelioMC uses [Conventional Commits](https://www.conventionalcommits.org/):

```text
<type>(<scope>): <short summary>

<optional body>

<optional footer>
```

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`,
`ci`, `chore`, `revert`.

Scopes: `auth`, `instance`, `launch`, `mod`, `modpack`, `content`, `settings`,
`ui`, `i18n`, `ci`, `docs`.

Examples:

```text
feat(instance): add top-level select alias
fix(auth): avoid Spectre markup crash in auth accounts
docs: document the Ely.by device-code flow
ci: publish linux-arm64 artifacts
```

Guidelines:

- Summary in the imperative mood, under 72 characters.
- Explain *why* in the body when the change is not obvious.
- Reference issues in the footer: `Closes #42`.
- Keep commits buildable. Squash fixups before requesting review if asked.

To use the included message template:

```bash
git config commit.template .gitmessage
```

### Branch names

Use a short prefix:

```text
feat/instance-select-alias
fix/auth-markup-crash
docs/elyby-oauth
ci/release-artifacts
```

## Pull request process

1. Fork the repository and create a branch from `main` (or `master`).
2. Make the change, build it, and test it.
3. Update docs, help text, translations, and completions together.
4. Open a pull request using the template. Fill in the testing section.
5. Keep the PR focused; respond to review comments with new commits.
6. A maintainer will squash-merge when approved.

A PR is ready when:

- `dotnet build -c Release` succeeds.
- Language JSON is valid.
- The affected command works, including its aliases and errors.
- No secrets or personal paths are included.
- Documentation and translations are updated.
- The diff contains no unrelated changes.

## Release process

Releases are automated by [`.github/workflows/release.yml`](.github/workflows/release.yml).

1. Bump `<Version>` in `Helio.csproj`.
2. Update `CHANGELOG.md`.
3. Commit: `chore(release): 1.2.3`.
4. Tag and push:

```bash
git tag v1.2.3
git push origin main --tags
```

The workflow verifies that the tag matches the project version, builds five
runtime identifiers, generates `SHA256SUMS.txt`, and creates the GitHub release.
If the tag and version differ, the release fails early.

## Security and secrets

- Never commit `accounts.json`, access tokens, refresh tokens, or passwords.
- Redact UUIDs, usernames, and local paths from logs if you prefer.
- Report vulnerabilities privately — see [SECURITY.md](SECURITY.md).
- If a token is exposed, sign in again with `helio auth elyby` to rotate it and
  remove the old account with `helio auth remove <alias>`.

## License

By contributing, you agree that your contribution may be distributed under the
project's license terms. See [LICENSE](LICENSE). If the upstream project adopts a
license, this fork will follow it.
