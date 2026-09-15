---
name: module-versioning
description: "Set an Intent Architect module's version via the Module Builder model property, then propagate it to dependents (architecture templates, other modules). USE ONLY WHEN asked to set, release, publish, or bump a module to a specific, already-decided version string. DO NOT USE FOR deciding whether or when a task should bump a version, or which component to bump (see module-version-increment) — this skill only executes a version already supplied. REQUIRES the target version string supplied by the caller; it does not decide or validate what it should be."
argument-hint: "[new version, e.g. 1.3.0 or 1.3.0-pre.1]"
keywords: [version, versioning, release, imodspec, module settings]
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleVersioning_SkillMd_Agents
contentHash: EC050EAE6B08B8A1CB1ED48F017F6E08F28E0CC4BCFDE27C965F1F2D4AADF2A9
---
# Skill: module-versioning

## What The Software Factory Owns

`.imodspec` is **not** a fully generated file. The template reads the existing file, rewrites a small
set of named elements, and leaves everything else exactly as it found it. So "never hand-edit
`.imodspec`" is wrong — most of that file is hand-authored by design.

- *The Software Factory owns these six. Everything else in the file is yours, permanently.**

| Element | Where to set it instead |
|---|---|
| `<version>` | Module Builder designer → `Module Settings` → `Version` |
| `<summary>` and `<description>` | The **Application Settings page** in Intent Architect. Both receive the *same single string* — there is no long-form field to expand into. |
| `<iconUrl>` | `.application.config`'s root `<icon>`/`<iconType>`, by script — see `module-svg-icon` |
| `<id>` | The Module Builder package name |
| `<migrations>` | Modelled Migration elements — deleted and rebuilt wholesale each run |
| `<moduleSettings>` bodies | Modelled settings elements — children wiped and rebuilt |

Hand-edit one of those six and the next Software Factory run silently discards it. Everything else —
`<tags>`, `<authors>`, `<files>`, `<dependency>` entries, `<interoperability>`, `<designers>` — the
template never writes a value for, so hand-editing is the *only* way to set them and the edit survives.

## How to Set It

1. On the module's package, in the Module Builder designer:

   
   `pkg.ensureStereotype("Module Settings").setProperty("Version", "<supplied version>")`
   (or the designer UI). Use the version exactly as supplied.

2. Run the Software Factory to regenerate `.imodspec`'s `<version>`.
3. Confirm via `get_file_diffs` that only the version line changed.

> **`<version>` is written only when the designer's value sorts strictly higher than the one on disk.**
> A lower value is skipped in silence — nothing staged, no error, no warning. That asymmetry is the
> downgrade guard `module-version-increment` describes, and it is the one case where editing the
> `<version>` line by hand is correct: drop it to a safe value, then set the version you actually want
> in the designer and regenerate forward.

## Propagating the Change

- **Architecture Templates** referencing this module — update the `Component Module`'s

  `Version` in `metadata.iatspec`, only once the new version is actually published (confirm via
  `search_available_modules`, never guess).

- **Other modules depending on it** — update their `<dependency id="..." version="...">` entry. That is

  propagation of a version you already know about; confirming the dependency *list itself* is right is a
  separate close-out check — see `module-dependency-audit`.

- **NuGet package alignment** — keep `.csproj` package versions in step with the module version

to avoid `NU1605` (see known-build-gotchas).

## After Setting the Version

Run `module-docs` to keep `release-notes.md` and other version-dependent documentation in sync —
this skill sets the version, it doesn't own the documentation that mirrors it.

## Verification Checklist

- [ ] Set via `Module Settings → Version` — not by hand-editing `.imodspec`'s `<version>` (except to clear a downgrade)
- [ ] Software Factory run; `.imodspec` confirmed to match, no stray diffs
- [ ] Dependents checked/updated if they pin this module's version
- [ ] `module-docs` run afterward to keep documentation in sync
