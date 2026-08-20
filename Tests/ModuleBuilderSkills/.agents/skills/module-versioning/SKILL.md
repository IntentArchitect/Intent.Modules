---
name: module-versioning
description: "Set an Intent Architect module's version correctly via the Module Builder model property (never by hand-editing .imodspec), then propagate it to dependents (architecture templates, other modules). USE ONLY WHEN asked to set, release, publish, or bump a module to a specific, already-decided version string. DO NOT USE FOR deciding whether or when a task should bump a version, or which component to bump (see module-version-increment) — this skill only executes a version already supplied. REQUIRES the target version string supplied by the caller; it does not decide or validate what it should be."
argument-hint: "[new version, e.g. 1.3.0 or 1.3.0-pre.1]"
keywords: [version, versioning, release, imodspec, module settings]
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleVersioning_SkillMd_Agents
contentHash: EEB88287536B99E4A584FB3D841DC6E3CBE53BE37FA555BA7D1DF1646072CB42
---
# Skill: module-versioning

## The Core Trap

`.imodspec`'s `<version>` (and `<description>`) are generated FROM the model on every Software
Factory run. Hand-editing `.imodspec` directly is silently reverted on the next run.

## How to Set It

1. On the module's package, in the Module Builder designer:

   
   `pkg.ensureStereotype("Module Settings").setProperty("Version", "<supplied version>")`
   (or the designer UI) — never the `.imodspec` file. Use the version exactly as supplied.

2. Run the Software Factory to regenerate `.imodspec`'s `<version>`.
3. Confirm via `get_file_diffs` that only the version line changed.

## Propagating the Change

- **Architecture Templates** referencing this module — update the `Component Module`'s

  `Version` in `metadata.iatspec`, only once the new version is actually published (confirm via
  `search_available_modules`, never guess).

- **Other modules depending on it** — update their `<dependency id="..." version="...">` entry.
- **NuGet package alignment** — keep `.csproj` package versions in step with the module version

to avoid `NU1605` (see known-build-gotchas).

## After Setting the Version

Run `module-docs` to keep `release-notes.md` and other version-dependent documentation in sync —
this skill sets the version, it doesn't own the documentation that mirrors it.

## Verification Checklist

- [ ] Set via `Module Settings → Version`, never by hand-editing `.imodspec`
- [ ] Software Factory run; `.imodspec` confirmed to match, no stray diffs
- [ ] Dependents checked/updated if they pin this module's version
- [ ] `module-docs` run afterward to keep documentation in sync
