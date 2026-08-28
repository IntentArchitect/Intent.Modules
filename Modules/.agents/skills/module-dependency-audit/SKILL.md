---
name: module-dependency-audit
description: "Verify a module's .imodspec dependencies match what its code and templates actually reference, and supply the ones the Software Factory did not detect. USE ONLY WHEN closing out a module change — this is Phase 4 step 2 of the module-building workflow, run after the version is confirmed and before documentation. DO NOT USE FOR bumping the module's own version (see module-version-increment), or for editing modules.config, which is never hand-edited. REQUIRES the module's implementation complete and its build green."
keywords: [dependencies, imodspec, install, close-out, verification, module]
template-id: Intent.ModuleBuilder.AI.Workflow.Skills.ModuleDependencyAudit_SkillMd_Agents
contentHash: B492F6811AE81EFDFA3CC7D14757BE3E7764F7D30CAE4A10341B3D1D1DEB5C1E
---
# Skill: module-dependency-audit

## Why This Is A Gate, Not An Errand

- *A missing dependency compiles perfectly and fails at install** — in a consumer's application, not yours.

The module builds, the Software Factory runs, the templates emit what you expected, and nothing goes wrong
until someone else installs it and the type it needs is not there.

That is the exact failure class the workflow's *"compiling is not working"* rule exists for. A green build
is not evidence here, so no earlier phase catches it. This check is why the manifest is trustworthy.

## What The Software Factory Already Does

`<dependencies>` is computed, not hand-written. The template assembles it from the modules the application
has installed, the dependency events templates raise while generating, and a small fixed set every module
needs. For the ordinary case it is right, and you should not be second-guessing it entry by entry.

- *It is a heuristic, and the source acknowledges as much.** It infers from what is *installed* rather than

from what the code actually *references*, so the gap is systematic rather than random:

- A type reached through a transitively-available assembly — present at compile time because something else

pulled it in, never declared as this module's own dependency.

- A dependency that was real when a template was written and is now only referenced from a code path the

generator no longer visits.

- Custom modules generally, where the installed set and the referenced set drift apart fastest.

## The Audit

1. **Collect what the module actually references.** Its `.csproj` `PackageReference` entries, the

`Intent.*` namespaces its templates and factory extensions import, and any designer metadata it reads.

2. **Read the generated `<dependencies>`** in the module's `.imodspec`.
3. **Compare, and care about one direction.** Something referenced but *not* declared is the defect — that

is the install-time failure. An entry that looks surplus is usually a legitimate transitive or
event-raised dependency; leave it unless you can show nothing needs it.

4. **Report what you checked**, not just that it passed. Name the references you compared against.

## Fixing What You Find

- *Fix it at the source first.** A dependency missing from the manifest is normally missing because the

module reference itself is missing or metadata-only in a way that hides it. Correct the reference — install
the module properly, add the `PackageReference` — and regenerate; the manifest then computes correctly and
stays correct on the next run.

- *A hand-added `<dependency>` is legitimate when the source fix does not reach it.** Unlike most of the

manifest, dependency entries are *added if absent and never pruned* — so an entry you add by hand survives
regeneration indefinitely. This is a supported outcome, not a hack. Use it when a genuinely required
dependency still does not appear after the reference is correct and the module has been regenerated.

Match the id form exactly — the Intent module id, which is not the NuGet package id — and the version to
what the module actually requires.

## Two Things This Skill Does Not Touch

- **`modules.config`** — never hand-edited, for any reason. It records what is installed; a bad edit

corrupts the application's module state. Change it by installing or updating through Intent Architect.

- **`supportedClientVersions`** — scaffolded once and then advanced automatically as the client SDK moves

forward. A range being rewritten to a newer one is the mechanism working, not drift. Look at it only if a
Software Factory run actually complains about client-version compatibility.

## Checklist

- [ ] Referenced set collected from `.csproj`, template imports, and designer metadata reads
- [ ] Generated `<dependencies>` compared against it; every referenced-but-undeclared entry resolved
- [ ] Fixes made at the source and regenerated — hand-added entries only where that did not reach
- [ ] Surplus-looking entries left alone unless shown to be unused
- [ ] `modules.config` untouched; `supportedClientVersions` untouched
- [ ] What was compared is stated in the close-out, not just that the check ran
