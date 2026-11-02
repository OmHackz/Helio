# Security Policy

## Supported versions

HelioMC is a small fork and only the latest published release (and the current
`main` branch) receives fixes.

| Version | Supported |
|---|---|
| Latest release | ✅ |
| `main` / `master` | ✅ |
| Older releases | ❌ |

## Reporting a vulnerability

**Please do not open a public issue for security problems.**

Report privately through GitHub:

1. Go to the repository's **Security** tab.
2. Choose **Advisories → Report a vulnerability**.
3. Describe the issue, affected versions, and reproduction steps.

Direct link once the repository exists:

<https://github.com/OmHackz/Helio/security/advisories/new>

If private reporting is unavailable, contact a maintainer through their GitHub
profile and ask for a private channel. Do not include exploit details in a
public message.

## What to include

- HelioMC version (`helio version`) and platform
- A clear description of the vulnerability and its impact
- Reproduction steps or a proof of concept
- Whether the issue requires a malicious server, mod, modpack, or local access
- Any suggested fix or mitigation

## What not to include

- Access tokens, refresh tokens, or `accounts.json` contents
- Passwords or 2FA codes
- Other users' personal data
- Public disclosure before a fix is available

If you accidentally expose a token, sign in again with
`helio auth elyby` to rotate it, then remove the old account with
`helio auth remove <alias>`.

## Scope

In scope:

- Token handling, refresh, and storage in `Services/AuthService.cs`
- Ely.by OAuth device-code flow and authlib-injector download/verification
- Command injection, path traversal, or unsafe file handling
- Modpack / mod archive extraction (zip-slip, arbitrary writes)
- Remote code execution through downloaded content
- Secrets written to logs or crash reports

Out of scope:

- Vulnerabilities in Minecraft, Java, Modrinth, Ely.by, or third-party mods
- Issues that require an already-compromised machine
- Social engineering of maintainers
- Denial of service through enormous downloads, unless it bypasses a safety
  check in HelioMC

## Response process

- Acknowledgement within **7 days** where possible.
- An initial assessment and severity estimate after triage.
- A fix and coordinated disclosure once a patch is ready.
- Credit in the release notes if you would like it.

This is a volunteer project, so timelines are best-effort. Thank you for
reporting responsibly.

## Safe defaults for users

- Download builds only from the project's Releases page.
- Verify `SHA256SUMS.txt`.
- Never share `accounts.json` or paste tokens into issues.
- Keep Java and HelioMC up to date.
- Treat modpacks and mods from unknown sources with the same caution you would
  apply to any executable content.
