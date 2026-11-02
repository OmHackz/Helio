# Changelog

All notable changes to HelioMC are documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added

- GitHub Actions workflows for CI builds, tagged releases, CodeQL analysis, and
  stale-issue cleanup.
- Issue forms, pull-request template, commit message template, and repository
  setup checklist.
- Top-level `select` / `use` shortcuts for `instance select`.
- This changelog, contribution guide, security policy, support guide, and code
  of conduct.

## [1.0.6] — 2026-11-02

### Added

- Native **Ely.by** account support.
  - OAuth 2.0 device-code sign-in (`helio auth elyby`).
  - Automatic token refresh with long-lived Ely.by tokens.
  - Password/Yggdrasil sign-in as an explicit fallback
    (`helio auth elyby --password`).
  - authlib-injector download, caching, and SHA-256 verification.
  - Provider-aware account status and listing.
- **HelioMC rebrand** from the upstream McSH project.
  - CLI and binary renamed to `helio`.
  - C# namespace and project renamed to `Helio` / `Helio.csproj`.
  - New `HELIO` startup banner with an orange gradient.
  - New data directory (`~/.local/share/Helio`) with a one-time migration from
    the legacy McSH directory.
- New `helio` colour theme with an orange accent, set as the default.
- Instance deselection: `instance deselect`, plus `unselect`, `none`, and `off`
  aliases and a top-level `deselect` shortcut.
- Top-level `select` and `use` shortcuts for the active instance.
- GitHub Actions release automation producing Linux x64/arm64, macOS x64/arm64,
  and Windows x64 artifacts with checksums.

### Changed

- Default theme migrated from `crimson` to `helio` once for existing installs.
- Help text, completions, version output, and documentation now use HelioMC
  branding.
- `helio update` reports that self-update is disabled for this local fork.
- Upstream release checks were removed from startup.
- User-Agent strings changed to `Helio-Launcher`.

### Fixed

- Spectre markup crash in `auth accounts` and `auth logout` caused by an
  unclosed markup tag.
- Quick-launch commands now load the persisted session, so `helio deselect`
  works outside the REPL.
- Quick-select prompt no longer shows a stale instance name after deselection.

### Removed

- Upstream self-update and release-notification behavior.

## Upstream history

HelioMC is a fork of [McSH](https://github.com/kalakaritzu/mcsh). For changes
before the fork, see the upstream repository.

[Unreleased]: https://github.com/OWNER/REPO/compare/v1.0.6...HEAD
[1.0.6]: https://github.com/OWNER/REPO/releases/tag/v1.0.6
