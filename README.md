<!--
  Repository setup: this project is prepared for a brand-new GitHub repository.
  Replace OWNER/REPO in this file (and in .github/ISSUE_TEMPLATE/config.yml,
  CODE_OF_CONDUCT.md, SECURITY.md, and SUPPORT.md) after creating it, e.g.:

    grep -rl 'OWNER/REPO' . --exclude-dir=.git | xargs sed -i 's|OWNER/REPO|your-name/heliomc|g'

  See .github/SETUP.md for the full checklist.
-->

<div align="center">

# HelioMC

**Minecraft in your terminal. No Electron. No splash screens. Just `helio`.**

[![Build](https://github.com/OWNER/REPO/actions/workflows/build.yml/badge.svg)](https://github.com/OWNER/REPO/actions/workflows/build.yml)
[![Release](https://github.com/OWNER/REPO/actions/workflows/release.yml/badge.svg)](https://github.com/OWNER/REPO/actions/workflows/release.yml)
[![CodeQL](https://github.com/OWNER/REPO/actions/workflows/codeql.yml/badge.svg)](https://github.com/OWNER/REPO/actions/workflows/codeql.yml)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Platforms](https://img.shields.io/badge/platforms-Linux%20%7C%20macOS%20%7C%20Windows-orange)](#install)
[![License](https://img.shields.io/badge/license-see%20LICENSE-lightgrey)](LICENSE)

```text
██╗  ██╗███████╗██╗     ██╗ ██████╗
██║  ██║██╔════╝██║     ██║██╔═══██╗
███████║█████╗  ██║     ██║██║   ██║
██╔══██║██╔══╝  ██║     ██║██║   ██║
██║  ██║███████╗███████╗██║╚██████╔╝
╚═╝  ╚═╝╚══════╝╚══════╝╚═╝ ╚═════╝
```

[Features](#features) · [Install](#install) · [Quick start](#quick-start) · [Commands](#command-reference) · [Ely.by](#elyby-sign-in) · [Build](#building-from-source) · [Contributing](#contributing)

</div>

---

**HelioMC** is a terminal-first Minecraft launcher for people who would rather
stay in a shell than click through a launcher. It manages instances, mods,
modpacks, worlds, Java, and accounts — including native
[Ely.by](https://ely.by) sign-in through browser OAuth.

It is an unofficial fork of [McSH](https://github.com/kalakaritzu/mcsh),
fully rebranded and extended with Ely.by support, an orange default theme, and
a few quality-of-life shortcuts.

> HelioMC is not affiliated with Mojang, Microsoft, Ely.by, or the upstream
> McSH project.

## Features

| | Feature | What it does |
|---|---|---|
| 🟠 | **Orange by default** | The `helio` theme ships as the default accent, with six more themes available. |
| 🔐 | **Ely.by OAuth** | `helio auth elyby` uses the device-code flow — approve in your browser, never type a password into the launcher. |
| 🧱 | **Instances** | Isolated versions, mods, worlds, RAM, window size, JVM args, hooks, and environment variables. |
| 🧩 | **Mods & modpacks** | Search Modrinth, install with dependencies, toggle, update, and snapshot loadouts with mod profiles. |
| 🎨 | **Content packs** | Resource packs, shaders, server plugins, and data packs from Modrinth. |
| ⚙️ | **Managed Java** | Detects installed JDKs and downloads a suitable one when needed. |
| 🖥️ | **Live console** | Attach to a running instance with `helio console`, detach with `Esc`. |
| 🚀 | **Quick launch** | `helio run <name>` starts an instance without entering the REPL. |
| 🕘 | **Recent worlds** | Jump back into a recent world or server across all instances. |
| 🧾 | **Scriptable** | Every command works non-interactively: `helio select MyPack && helio launch`. |
| 🌍 | **English & Spanish** | Switch languages with `helio settings language <en\|es>`. |
| 📦 | **Self-contained** | Published builds need no .NET runtime and no installer. |

## A shell-native launcher

```text
$ helio
> auth elyby
  Open https://account.ely.by/code and enter: ABCD-1234
  ✔ Signed in as Omhackz

> instance create
  ✔ Created "Fabulous 1.21" (Fabric 1.21.1)

> select "Fabulous 1.21"
  ✔ Set to Fabulous 1.21.

> mod search sodium
  ...
> mod install 1

> launch
  ✔ Launching Fabulous 1.21...
```

The active instance is shown in the prompt:

```text
> [Fabulous 1.21]:
```

Unselect it whenever you want a clean prompt:

```text
> [Fabulous 1.21]: deselect
  ✔ Deselected Fabulous 1.21.
>
```

## Install

### Prebuilt binaries

Download the archive for your platform from
[Releases](https://github.com/OWNER/REPO/releases/latest).

| Platform | Asset | Binary |
|---|---|---|
| Linux x64 | `helio-<version>-linux-x64.tar.gz` | `helio` |
| Linux arm64 | `helio-<version>-linux-arm64.tar.gz` | `helio` |
| macOS Apple Silicon | `helio-<version>-osx-arm64.tar.gz` | `helio` |
| macOS Intel | `helio-<version>-osx-x64.tar.gz` | `helio` |
| Windows x64 | `helio-<version>-win-x64.zip` | `helio.exe` |

Every release also includes `SHA256SUMS.txt`; verify before running anything
you downloaded.

**Linux / macOS**

```bash
tar -xzf helio-*-<platform>.tar.gz
chmod +x helio
sudo install -m755 helio /usr/local/bin/helio
helio version
```

Without `sudo`, install to your user path instead:

```bash
mkdir -p ~/.local/bin
install -m755 helio ~/.local/bin/helio
export PATH="$HOME/.local/bin:$PATH"
```

**Windows (PowerShell)**

```powershell
Expand-Archive .\helio-*-win-x64.zip -DestinationPath $env:LOCALAPPDATA\Helio
# Add the folder to your user PATH, then open a new terminal:
helio version
```

### From source

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone https://github.com/OWNER/REPO.git
cd REPO

# Pick the runtime identifier for your machine:
dotnet publish Helio.csproj -c Release -r linux-x64 --self-contained true -o out
# linux-arm64 · osx-arm64 · osx-x64 · win-x64

sudo install -m755 out/helio /usr/local/bin/helio
```

On Windows:

```powershell
dotnet publish Helio.csproj -c Release -r win-x64 --self-contained true -o out
.\out\helio.exe version
```

See [BUILD.md](BUILD.md) for fork-specific build notes.

## Quick start

```bash
helio auth elyby              # sign in with Ely.by (browser OAuth)
helio instance create         # interactive instance wizard
helio select "My Instance"    # set the active instance
helio launch                  # launch it
```

Or do the whole thing without the REPL:

```bash
helio auth elyby && helio instance create && helio select "My Instance" && helio launch
```

## Ely.by sign-in

HelioMC uses Ely.by's **OAuth 2.0 device-code flow**. No password is sent to the
launcher, and no client secret is required.

```bash
helio auth elyby            # default alias
helio auth elyby personal   # save under a custom alias
```

HelioMC prints a short code and opens `https://account.ely.by/code`. Approve the
request, and the account is saved to
`~/.local/share/Helio/accounts.json`.

The flow requests these scopes:

- `account_info` — read your Ely.by profile
- `minecraft_server_session` — the token Minecraft uses to join servers
- `offline_access` — refresh the token without signing in again

Tokens are long-lived (Ely.by currently issues roughly 11-year tokens) and
refresh automatically. **Never share `accounts.json` or paste tokens into an
issue.**

To use your own registered Ely.by application instead of the public `ely`
client:

```bash
export HELIO_ELYBY_CLIENT_ID=your_client_id
helio auth elyby --force
```

### Password fallback

If device-code sign-in is unavailable, the legacy Yggdrasil/password path is
kept as an explicit fallback:

```bash
helio auth elyby --password
```

For accounts with 2FA, enter the password as `password:2facode`. The password is
never stored.

### Account commands

```text
helio auth status              # active account, UUID, token expiry
helio auth accounts            # list saved accounts
helio auth switch <alias>      # change the active account
helio auth remove <alias>      # delete a saved account
helio auth logout              # sign out of the active account
```

## Command reference

Run `helio help` for the quick-start guide or `helio ref` for the full list.
Every command also works as a one-shot: `helio <command>`.

### Instances

| Command | Description |
|---|---|
| `instance list` | List instances with version and loader |
| `instance create` | Interactive creation wizard |
| `instance select <name>` | Set the active instance (alias: `select`, `use`) |
| `instance deselect` | Unselect the active instance (aliases: `deselect`, `unselect`, `none`, `off`) |
| `instance run [name]` | Launch the active instance, or the named one (alias: `run`, `launch`) |
| `instance stop [name]` | Force-terminate a running instance (alias: `kill`) |
| `instance delete <name>` | Delete an instance folder (`rm`, `remove`) |
| `instance rename <old> <new>` | Rename an instance |
| `instance clone <name>` | Duplicate an instance |
| `instance update [name]` | Update all mods to their latest versions |
| `instance worlds [name]` | List worlds; back up, delete, or open them |
| `instance screenshots [name]` | Browse screenshots |
| `instance crash [name]` | Show the latest crash report |
| `instance config [name]` | Configure RAM, window, JVM args, env vars, hooks |
| `instance import <path.mrpack>` | Import a Modrinth modpack |
| `instance export [name]` | Zip the full instance folder |
| `instance backup [name]` | Zip world saves with a timestamp |
| `instance mrpack [name] [version]` | Export as a shareable `.mrpack` |
| `instance prism <path>` | Import a Prism Launcher / MultiMC instance (`multimc`) |

### Mods and content

| Command | Description |
|---|---|
| `mod search <query>` | Search Modrinth for the active instance's version |
| `mod install <id>` | Install a mod and its dependencies |
| `mod remove <name>` | Remove a mod (`uninstall`, `rm`) |
| `mod details <id\|#>` | Show a mod, then optionally install it |
| `mod import <path>` | Import a local `.jar` |
| `mod toggle <name>` | Enable or disable without deleting |
| `mod open` | Open the mods folder |
| `mod list` | List installed mods |
| `mod profile save\|load\|list\|delete` | Snapshot and restore mod loadouts |
| `modpack search <query>` | Search Modrinth for modpacks (`mp`) |
| `modpack install <#\|slug>` | Install a modpack as a new instance |
| `modpack update` | Check installed modpacks for newer versions |
| `resourcepack search\|install\|details` | Resource packs (`rp`) |
| `shader search\|install\|details` | Shader packs |
| `plugin search\|install\|details` | Server plugins (`pl`) |
| `datapack search\|install\|details` | Data packs (`dp`) |

### Accounts, skins, and system

| Command | Description |
|---|---|
| `auth elyby [alias]` | Ely.by browser OAuth |
| `auth elyby --password [alias]` | Ely.by password fallback |
| `auth status` / `auth accounts` | Inspect accounts |
| `auth switch <alias>` / `auth remove <alias>` / `auth logout` | Manage accounts |
| `skin` | List and apply saved skins |
| `skin import <path.png>` | Import a skin PNG |
| `skin cape` | Select or disable a cape |
| `skin delete <name>` | Delete a saved skin |
| `java` | List detected JDKs; set a custom path |
| `console` | Attach to the running instance console (`Esc` to detach) |
| `recent` / `recent run <#>` | Recently played worlds and servers |
| `settings` | Interactive settings menu |
| `settings theme [name]` | Switch colour theme |
| `settings language <en\|es>` | Switch language |
| `settings recent <count>` | Recent entries on startup (`0` = off) |
| `help` / `ref` | Quick help / full reference |
| `version` / `restart` / `clear` / `exit` | System commands |

> `helio update` is intentionally disabled. HelioMC is a local fork — update by
> pulling and rebuilding, or by downloading a newer release.

## Instances and the active selection

An instance groups a Minecraft version, loader, mods, worlds, and launch
settings. One instance can be **active**, which lets you omit its name from most
commands.

```bash
helio select "Fabulous 1.21"       # top-level shortcut
helio instance select "Fabulous 1.21"
helio use "Fabulous 1.21"          # alias

helio deselect                     # top-level shortcut
helio instance deselect            # aliases: unselect, none, off
```

The selection is stored in `session.json` and restored on the next start. If
the instance no longer exists, HelioMC ignores the stale entry. Run `deselect`
if you prefer a plain `> ` prompt.

## Themes

The default theme is **`helio`**, an orange accent.

```text
helio settings theme helio      # orange (default)
helio settings theme crimson
helio settings theme arctic
helio settings theme forest
helio settings theme amber
helio settings theme violet
helio settings theme steel
```

Themes also change the startup banner gradient.

## Data and configuration

HelioMC stores everything in one directory:

| Platform | Path |
|---|---|
| Linux | `~/.local/share/Helio/` (respects `$XDG_DATA_HOME`) |
| macOS | `~/Library/Application Support/Helio/` |
| Windows | `%APPDATA%\Helio\` |

| File / folder | Contents |
|---|---|
| `accounts.json` | Saved accounts and tokens — **private** |
| `session.json` | Active instance |
| `settings.json` | Theme, language, prompt, and UI preferences |
| `instances/` | Instance folders (mods, worlds, configs) |
| `versions/`, `libraries/`, `assets/` | Shared Minecraft files |
| `authlib-injector.jar` | Cached Ely.by authlib-injector, SHA-256 verified |

On first run, an existing `McSH` data directory is **moved** to the Helio
location so instances and accounts carry over. Back up the folder before
experimenting.

To reset HelioMC, quit it and rename the data directory — do not delete it until
you have copied anything you need.

## Building from source

Prerequisites: .NET 8 SDK, and network access for NuGet restore.

```bash
dotnet restore Helio.csproj
dotnet build Helio.csproj -c Release
dotnet bin/Release/net8.0/helio.dll version
```

Self-contained publish:

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

### Project layout

```text
.
├── Commands/          # One class per command group
├── Models/            # Settings, accounts, instances, caches
├── Services/          # Auth, Modrinth, Java, launch, themes, paths
├── State/             # REPL state and session persistence
├── Languages/         # en.json, es.json
├── Properties/        # Windows publish profile
├── Program.cs         # Entry point, REPL loop, completions
├── UiTheme.cs         # Theme definitions
├── Helio.csproj       # Project file (net8.0, assembly "helio")
├── BUILD.md           # Fork-specific build notes
└── CONTRIBUTING.md    # Development and PR guide
```

### Releases

The GitHub Actions workflows are ready to use:

- **Build** (`build.yml`) — restores, builds, validates language JSON, runs a
  smoke test, and publishes artifacts for five runtime identifiers on every
  push and pull request.
- **Release** (`release.yml`) — triggered by a `v*` tag. It verifies that the
  tag matches `<Version>` in `Helio.csproj`, builds every platform, generates
  `SHA256SUMS.txt`, and creates a GitHub release.
- **CodeQL** (`codeql.yml`) — weekly and per-change security analysis.
- **Stale** (`stale.yml`) — keeps the issue tracker tidy.

To cut a release:

```bash
# 1. Bump <Version> in Helio.csproj
# 2. Commit the change
git commit -am "chore(release): 1.0.7"
git tag v1.0.7
git push origin main --tags
```

## Troubleshooting

**Minecraft launches and closes immediately**
Run `helio instance crash` for the newest crash report, then check the Java
version. `helio java` lists what HelioMC found; game version and loader must
match a supported JDK.

**Ely.by sign-in keeps waiting**
Open `https://account.ely.by/code` manually and confirm the code. If you set
`HELIO_ELYBY_CLIENT_ID`, unset it to use the public `ely` client, then retry.

**`auth status` says the token expired**
Tokens refresh automatically at startup. If refresh fails, run
`helio auth elyby` again. Never share the old token.

**macOS says the binary cannot be opened**
Remove the quarantine attribute after verifying the checksum:

```bash
xattr -d com.apple.quarantine ./helio
chmod +x ./helio
```

**Windows SmartScreen warns about the executable**
The release binaries are unsigned. Verify `SHA256SUMS.txt`, then choose
*More info → Run anyway* if you trust the build.

**Wrong prompt / stale instance name**
Run `helio deselect`. The selection lives in `session.json`.

## Contributing

Contributions are welcome — code, translations, docs, themes, and bug reports.

- Read [CONTRIBUTING.md](CONTRIBUTING.md).
- Follow the [Code of Conduct](CODE_OF_CONDUCT.md).
- Use the issue and pull-request templates.
- Report vulnerabilities privately via [SECURITY.md](SECURITY.md).
- Ask questions in [SUPPORT.md](SUPPORT.md).

Good first issues are labelled `good first issue`.

## License

See [LICENSE](LICENSE). HelioMC is a fork of McSH by
[kalakaritzu](https://github.com/kalakaritzu/mcsh); the upstream project does not
currently declare a license, so this fork does not grant additional rights.
Confirm you have the necessary rights before redistributing.

## Acknowledgements

- [McSH](https://github.com/kalakaritzu/mcsh) — the upstream terminal launcher.
- [Spectre.Console](https://spectreconsole.net/) — terminal UI.
- [Modrinth](https://modrinth.com/) — mod, modpack, and content APIs.
- [Ely.by](https://ely.by) — account provider and authlib-injector.
- [authlib-injector](https://github.com/yushijinhun/authlib-injector) — Yggdrasil
  compatibility layer.
