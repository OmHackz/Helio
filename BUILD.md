# HelioMC — build notes

HelioMC is an unofficial fork of [McSH](https://github.com/kalakaritzu/mcsh)
with a full rebrand and native [Ely.by](https://ely.by) account support.

For the user-facing guide, see [README.md](README.md). For contribution
workflow, see [CONTRIBUTING.md](CONTRIBUTING.md). For a code map, see
[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Fork changes

### Rebrand

- Project renamed to **HelioMC**, CLI/binary renamed to **`helio`**.
- C# namespace/root namespace changed from `McSH` to `Helio`.
- Assembly name changed from `mcsh` to `helio`.
- Data directory changed from `McSH` to `Helio`
  (`~/.local/share/Helio` on Linux), with a one-time migration that moves an
  existing McSH data directory.
- Banner logo and all help/UI strings updated to HelioMC.
- User-Agent strings changed to `Helio-Launcher`.
- Upstream release checks removed; `update` reports that self-update is
  disabled.
- Default theme is `helio` (orange accent); existing McSH settings that used
  the old default `crimson` are migrated to `helio` once.
- Added theme: `helio` (orange). Existing themes are unchanged.
- Added `select` / `use` and `deselect` / `unselect` / `none` shortcuts.

### Ely.by support

- `Models/AuthTokens.cs` — `Provider` (`microsoft` / `elyby`), `AuthMethod`
  (`oauth` / `password`), `OAuthRefreshToken`, `ClientToken`, `IsElyBy`,
  `IsElyByOAuth`.
- `Services/AuthService.cs`
  - **OAuth 2.0 device-code login** against Ely.by
    (`/api/oauth2/v1/devicecode` + `/api/oauth2/v1/token`), including
    `authorization_pending` / `slow_down` handling, automatic browser launch,
    and profile lookup via `/api/account/v1/info`.
  - OAuth refresh-token support.
  - Password/Yggdrasil login kept as an explicit fallback.
  - JWT expiry parsing and provider-aware status/listing.
- `Services/AuthlibInjectorService.cs` (new) — downloads and caches
  authlib-injector (`~/.local/share/Helio/authlib-injector.jar`), verifies its
  SHA-256, and reuses it.
- `Services/GameLaunchService.cs` — passes `${user_type}=mojang` and adds
  `-javaagent:.../authlib-injector.jar=ely.by` for Ely.by accounts.
- `Commands/AuthCommand.cs` — `auth elyby [--password] [alias]` and
  `auth login elyby [--password] [alias]`.
- `Commands/SkinCommand.cs` — points Ely.by users at account.ely.by instead of
  trying Mojang skin APIs.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Network access for NuGet restore
- Git (for cloning and releases)

## Local build

```bash
git clone https://github.com/OmHackz/Helio.git
cd REPO

dotnet restore Helio.csproj
dotnet build Helio.csproj -c Release
dotnet bin/Release/net8.0/helio.dll version
```

### Self-contained publish

```bash
dotnet publish Helio.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:EnableCompressionInSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o out
```

Supported runtime identifiers used by CI:

```text
linux-x64  linux-arm64  osx-x64  osx-arm64  win-x64
```

Install on Linux/macOS:

```bash
sudo install -m755 out/helio /usr/local/bin/helio
```

On Windows, run `out\helio.exe` or add `out` to your user `PATH`.

## GitHub Actions

The repository ships with four workflows:

| Workflow | Trigger | What it does |
|---|---|---|
| `build.yml` | push, PR, manual | Restores, builds, validates language JSON, smoke-tests `helio version`, publishes five RIDs as artifacts |
| `release.yml` | `v*` tag | Verifies tag vs `<Version>`, builds all platforms, writes `SHA256SUMS.txt`, creates the GitHub release |
| `codeql.yml` | push, PR, weekly | CodeQL security and quality analysis for C# |
| `stale.yml` | weekly, manual | Marks and closes inactive issues/PRs |

Artifacts are named:

```text
helio-<version>-linux-x64.tar.gz
helio-<version>-linux-arm64.tar.gz
helio-<version>-osx-x64.tar.gz
helio-<version>-osx-arm64.tar.gz
helio-<version>-win-x64.zip
SHA256SUMS.txt
```

### Cutting a release

1. Bump `<Version>` in `Helio.csproj`.
2. Update `CHANGELOG.md`.
3. Commit the change.
4. Tag and push:

```bash
git tag v1.0.6
git push origin main --tags
```

If the tag does not match the project version, the workflow fails before
publishing anything.

## Use

```text
helio                                # interactive REPL
helio auth elyby [alias]             # browser OAuth (device code)
helio auth elyby --password [alias]  # password/Yggdrasil fallback
helio instance create
helio select "My Instance"           # shortcut for instance select
helio launch
helio deselect                       # clear the active instance
```

`auth elyby` prints a short code and a URL (`https://account.ely.by/code`),
opens the page in your browser, and waits for approval. No password is entered
in the launcher.

If your Ely.by account has 2FA enabled and you use the `--password` fallback,
enter the password as `password:2facode`.

## OAuth client id

The device-code flow uses Ely.by's public client id `ely`, which needs no
client secret. To use your own registered application, set:

```bash
export HELIO_ELYBY_CLIENT_ID=your_client_id
```

Device-code flow does not need a redirect URI or client secret.

## Notes

- OAuth tokens are requested with the `account_info`,
  `minecraft_server_session`, and `offline_access` scopes. The token carrying
  `minecraft_server_session` is passed to Minecraft as the access token.
- Tokens are saved in `~/.local/share/Helio/accounts.json`; the password (when
  using the fallback) is never stored.
- `helio update` is disabled. Rebuild from source or download a newer release.
- HelioMC is an unofficial fork and is not affiliated with McSH, Mojang,
  Microsoft, or Ely.by.
