# Repository setup checklist

This project is prepared for a **brand-new GitHub repository**. Complete these
steps after creating it.

## 1. Set the repository slug

Links and badges use the placeholder `OWNER/REPO`. Replace it everywhere:

**Linux**

```bash
grep -rl 'OWNER/REPO' . --exclude-dir=.git --exclude-dir=bin --exclude-dir=obj \
  | xargs sed -i 's|OWNER/REPO|your-name/your-repo|g'
```

**macOS**

```bash
grep -rl 'OWNER/REPO' . --exclude-dir=.git --exclude-dir=bin --exclude-dir=obj \
  | xargs sed -i '' 's|OWNER/REPO|your-name/your-repo|g'
```

Files affected:

- `README.md`
- `CHANGELOG.md`
- `CONTRIBUTING.md`
- `CODE_OF_CONDUCT.md`
- `SECURITY.md`
- `SUPPORT.md`
- `.github/ISSUE_TEMPLATE/config.yml`

Verify:

```bash
grep -rn 'OWNER/REPO' . --exclude-dir=.git || echo "clean"
```

## 2. Repository settings

- **Description:** `A terminal-first Minecraft launcher with native Ely.by sign-in.`
- **Topics:** `minecraft`, `launcher`, `terminal`, `cli`, `tui`, `elyby`,
  `fabric`, `forge`, `neoforge`, `modrinth`, `dotnet`, `csharp`
- **Default branch:** `main` (or keep `master`; workflows support both)
- **Discussions:** enable
- **Private vulnerability reporting:** enable (Settings → Code security)
- **Dependency graph / Dependabot alerts:** enable

## 3. Labels

Create these labels so issue templates and automation apply them:

| Label | Colour | Purpose |
|---|---|---|
| `bug` | `#d73a4a` | Confirmed or reported defect |
| `enhancement` | `#a2eeef` | Feature request |
| `question` | `#d876e3` | Usage question |
| `needs triage` | `#fbca04` | Awaiting maintainer review |
| `confirmed` | `#0e8a16` | Reproduced by a maintainer |
| `good first issue` | `#7057ff` | Friendly entry point |
| `dependencies` | `#0366d6` | Dependabot updates |
| `stale` | `#808080` | Inactive issue or PR |
| `pinned` | `#000000` | Exempt from stale automation |
| `security` | `#b60205` | Security-related, avoid public detail |
| `work in progress` | `#f9d0c4` | Draft or incomplete PR |

## 4. Actions permissions

Settings → Actions → General:

- Allow all actions and reusable workflows (the workflows use official actions
  and `softprops/action-gh-release`).
- Workflow permissions: **Read repository contents and packages** by default.
  Release jobs request `contents: write` explicitly.

If your organization restricts third-party actions, allow:

- `actions/*`
- `github/codeql-action/*`
- `softprops/action-gh-release@v2`

## 5. Branch protection

For `main`:

- Require a pull request before merging.
- Require status checks: `Build & smoke test` and `Analyze C#`.
- Require branches to be up to date before merging.
- Block force pushes and deletions.
- Optionally require one approval.

## 6. First release

1. Confirm the workflows are on the default branch.
2. Check `<Version>` in `Helio.csproj` (currently `1.0.6`).
3. Tag and push:

```bash
git tag v1.0.6
git push origin main --tags
```

The release workflow verifies the tag, builds five artifacts, writes
`SHA256SUMS.txt`, and opens the release.

## 7. Licensing

Read [LICENSE](../LICENSE). Upstream McSH does not declare a license. Decide how
you want to license your own changes before publishing, and update `LICENSE` and
the README license section accordingly.

## 8. Optional polish

- Add a social preview image (Settings → Social preview).
- Pin a "start here" discussion.
- Add a `FUNDING.yml` if you accept sponsors.
- Enable GitHub Pages only if you add a docs site.
- Add a `CODEOWNERS` file if you want automatic review assignments:

```text
# .github/CODEOWNERS
* @your-github-handle
```
