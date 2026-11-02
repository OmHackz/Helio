# Architecture

How HelioMC is put together, for contributors who need a map before changing
code.

## Process model

HelioMC is a single console application. There is no daemon and no background
service.

```text
helio [args]
   │
   ├─ args present ──► quick-launch: build AppState + CommandRouter,
   │                   load session, dispatch one command, exit
   │
   └─ no args ──────► load settings, banner, load session,
                      start REPL, read line, dispatch, repeat
```

`Program.cs` owns both paths. The REPL adds tab completion, history, theming,
and the prompt; the quick-launch path reuses the same `CommandRouter`, so every
command behaves identically whether invoked interactively or from a script.

## Dispatch

`Commands/CommandRouter.cs` maps the first token to a command group:

| Verb | Handler |
|---|---|
| `instance`, `i`, `ins`, `inst` | `InstanceCommand` |
| `select`, `use` | `InstanceCommand` (`select`) |
| `deselect`, `unselect`, `none` | `InstanceCommand` (`deselect`) |
| `run`, `r`, `launch` | `InstanceCommand` (`run`) |
| `mod`, `m` | `ModCommand` |
| `modpack`, `mp` | `ModpackCommand` |
| `resourcepack`, `shader`, `plugin`, `datapack` | `ContentCommand` |
| `auth`, `a`, `login` | `AuthCommand` |
| `settings`, `set`, `s` | `SettingsCommand` |
| `recent` | `RecentCommand` |
| `skin`, `java`, `console` | dedicated commands |
| `help`, `ref`, `version`, `update`, `restart`, `clear`, `exit` | routed inline |

Command classes receive shared services in their constructor and own their
argument parsing. They print with Spectre.Console and return a `Task`.

## State and persistence

`State/AppState.cs` holds mutable REPL state:

- `ActiveInstance` — the selected instance name
- `LastSearchHits` — results used for install-by-number
- session persistence

Setting `ActiveInstance` clears stale search hits and writes `session.json`.
On startup, `LoadSession()` restores the name only if the instance manifest
still exists, so deleted instances never leave a broken prompt.

| File | Serialized by | Contains |
|---|---|---|
| `session.json` | `AppState` | active instance |
| `settings.json` | `SettingsService` | theme, language, UI preferences |
| `accounts.json` | `AuthService` | accounts and tokens (sensitive) |
| instance manifests | `InstanceStore` | version, loader, launch config |

`Services/PathService.cs` resolves the platform-specific root directory and
performs a one-time move from the legacy `McSH` directory.

## Authentication

`Services/AuthService.cs` supports two Ely.by paths.

### Device-code OAuth (default)

```text
launcher                     Ely.by
   │  POST /oauth2/v1/devicecode │
   │────────────────────────────►│
   │  user_code + verification_uri
   │◄────────────────────────────│
   │  open browser, poll token    │
   │  POST /oauth2/v1/token       │
   │────────────────────────────►│
   │  access + refresh token      │
   │◄────────────────────────────│
   │  GET /account/v1/info        │
   │────────────────────────────►│
   │  uuid + username             │
   │◄────────────────────────────│
```

The flow handles `authorization_pending` and `slow_down`, requests
`account_info`, `minecraft_server_session`, and `offline_access`, and refreshes
tokens transparently. The public client id `ely` is used unless
`HELIO_ELYBY_CLIENT_ID` overrides it. No client secret is required.

### Password fallback

`helio auth elyby --password` performs the legacy Yggdrasil authenticate and
refresh calls. The password is used once and never persisted. Accounts with 2FA
accept `password:2facode`.

### Launch integration

For Ely.by accounts, `Services/GameLaunchService.cs`:

- passes `${user_type}=mojang`
- adds `-javaagent:<cache>/authlib-injector.jar=ely.by`
- uses the Ely.by access token as the Minecraft session token

`Services/AuthlibInjectorService.cs` downloads the jar once, verifies its
SHA-256, and reuses the cached copy.

## Launch pipeline

1. Resolve the target instance and its manifest.
2. Resolve Java (`JavaService`), downloading a JDK if necessary.
3. Resolve the Minecraft version and loader (Vanilla, Fabric, Forge, NeoForge,
   Quilt) through the relevant service.
4. Ensure libraries and assets are present.
5. Build the command line from instance config, account, and JVM arguments.
6. Start the process and track it (`ProcessTracker`).
7. Optionally stream the console to `helio console`.

## Content

`Services/ModrinthService.cs` is the HTTP client. `ModStore` tracks installed
files, `ModProfile` snapshots loadouts, and `MrpackService` handles Modrinth
pack import/export. Content commands share `ContentCommand` with a type enum for
resource packs, shaders, plugins, and data packs.

## UI and localization

- `UiTheme.cs` defines named themes and the current accent markup.
- `Services/BannerService.cs` renders the ASCII banner and gradient.
- `Services/LanguageService.cs` loads `Languages/<code>.json` and falls back to
  the key, which makes missing translations visible.
- `Services/SettingsService.cs` applies theme and language at startup and
  performs one-time migrations (for example, `crimson` → `helio`).

## Extension points

To add a command:

1. Add a handler in the relevant `Commands/*.cs`.
2. Route the verb in `CommandRouter`.
3. Add help text in `HelpCommand` plus keys in both language files.
4. Add tab completions in `Program.cs`.
5. Document it in `README.md`.
6. Test interactive and one-shot invocation.

To add a theme:

1. Add a `ThemeDefinition` in `UiTheme.cs`.
2. Add a banner gradient in `BannerService.cs`.
3. Add the name to the settings completion list in `Program.cs`.
4. Update the theme list in the README and language files.

## Design constraints

- **No background services.** Commands are synchronous from the user's point of
  view; long work shows a spinner.
- **Never lose user data.** Migrations move or copy; deletes require explicit
  confirmation.
- **Provider-aware auth.** Do not assume Microsoft or Ely.by in shared code.
- **Small dependency surface.** Prefer the standard library and existing
  packages.
- **Terminal-first.** If a feature needs a GUI, it probably belongs elsewhere.
