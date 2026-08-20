---
name: module-docs
description: "Complete or refresh a module's release-notes.md, docs/README.md, and .imodspec metadata to a canonical format, touching only what already exists or what the maintainer explicitly supplies. USE ONLY WHEN a maintainer explicitly asks for documentation to be completed or brought up to this format. DO NOT USE FOR the automatic same-turn doc update after an observable change (see module-docs-chore) — this is an opt-in pass, and it must never introduce release-notes.md or fabricate a projectUrl unprompted. REQUIRES the target module's .imodspec (and any existing release-notes.md/docs/README.md) already present to read from."
keywords: [release-notes, readme, imodspec, documentation, module]
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleDocs_SkillMd_Agents
contentHash: 1381F53C4F62F7BB742DFA427709057E7472AD726B9EAE5633854BDA2FBFEBD2
---
# Skill: module-docs

## When to Use

Use this when a module maintainer wants documentation completed or updated to this format — it is optional, not an automatic step of every build.

- **`release-notes.md`** — only complete/maintain an entry if the file already exists, or the maintainer explicitly asks for one to be created. Never introduce it unprompted.
- **`docs/README.md`** — feature/usage documentation; fill gaps in an existing one, or write one if asked
- **`*.imodspec`** — metadata fields consumed by the module registry (summary, description, tags, releaseNotes, and `projectUrl` only if the maintainer supplies one)

===

## Step 1 — Gather Source Material

Before writing anything, read these files from the target module directory:

| File | What to extract |
|---|---|
| `*.imodspec` | Current `<version>` (or version in filename), existing `<summary>`, `<description>`, `<tags>`, `<projectUrl>` (if present), `<releaseNotes>` |
| `release-notes.md` | Existing version entries — determine what's already documented to avoid duplication |
| `docs/README.md` | Existing sections — fill gaps only, never overwrite sections that already have content |
| Template `*TemplatePartial.cs` files | Class names and roles — becomes the "What This Module Generates" list |
| Factory extension `*FactoryExtension.cs` files | What cross-cutting concerns they add (DI registrations, appsettings, template modifications) |
| `NugetPackages.cs` | NuGet packages the module introduces — informs tags and README dependency callouts |

===

## Artifact 1: `release-notes.md`

### Format

```markdown
### Version X.Y.Z

- New Feature: [description]
- Improvement: [description]
- Fixed: [description]

> ⚠️ **NOTE**
>
> [Breaking change explanation and migration guidance.]
```

### Rules

- **Heading:** `### Version X.Y.Z` — h3 only, "Version" prefix, version from `.imodspec` with `-pre.X` suffix **stripped** (e.g. `1.0.0-pre.0` → `### Version 1.0.0`)
- **Order:** Reverse chronological — newest entry at top
- **Prefixes:** Exactly three — `New Feature:`, `Improvement:`, `Fixed:` — no others
- **Bullets:** Single-line, 10–40 words, terse and factual; no multi-line or nested bullets
- **No prose** outside bullets — no introductory text, no dates, no metadata
- **Breaking changes:** `> ⚠️ **NOTE**` blockquote directly under the version heading, before bullets
- **Style:** Neutral, professional, no marketing language, no exclamation marks
- **Formatting:** Backticks for class/method/stereotype/setting names; plain text elsewhere

### What to write

- `New Feature:` — one entry per template or major capability added
- `Improvement:` — NuGet version bumps, dependency updates, refactors, performance
- `Fixed:` — only for known bugs resolved; include the scenario that triggered the bug
- For a first release (pre.0), write `New Feature:` entries covering each template/feature

### Example (first release)

```markdown
### Version 1.0.0

- New Feature: Added `NServiceBusConfiguration` template that scaffolds NServiceBus endpoint configuration with transport, serialization, and DI setup.
- New Feature: Added `NServiceBusMessageBus` infrastructure implementation wiring `IMessageBus` to NServiceBus send/publish operations.
- New Feature: Added support for SQL Persistence outbox pattern when `OutboxPattern` is set to `EntityFramework` on the application settings.
```

===

## Artifact 2: `docs/README.md`

### Canonical heading structure

```markdown
# [Module.Name]

[1-2 sentence purpose statement: what pattern/technology + what the module generates.]

## What is [Technology/Pattern]?
[Explanation of the external technology. Link to official docs. Omit if the technology is well-known and self-explanatory.]

## What This Module Generates
[Bullet list of file/class names the module produces.]

## [Feature Section — one H2 per major capability]
[Explain the pattern/concept. Show modeled input → generated output with C# code examples.]

### [Sub-concept — H3 for detailed breakdowns]

## Module Settings
### [Setting Group Name]
[Describe what the setting controls and its effect on generated code. Not just the name.]

## Related Modules
### [Module Name](link)
[1-2 sentences: what this module provides and how it integrates with the documented module.]

## External Resources
[Links to official docs, GitHub, specification pages. Optional.]
```

### Rules

- **H1** = module name only — no tagline
- **Opening paragraph:** one conceptual sentence ("This module…") + one artifact sentence ("It generates…")
- **`What This Module Generates`**: bullet list of class/file names; include the role each plays
- **Feature sections:** explain the concept first, then show the generated C# with syntax-highlighted code blocks
- **Code examples:** show both modeled input (what you configure in the designer) and generated output where useful
- **Images:** `![Alt text](images/filename.png)` only — never embed binary content
- **Notes/warnings:** `> [!NOTE]` / `> [!WARNING]` syntax
- **`Module Settings`:** H3 per setting group; describe effect on generated code, not just the setting name
- **`Related Modules`:** always present as the last content section; H3 per module; explain the integration relationship
- **Do not include:** Installation, Changelog, API Reference, Table of Contents, or troubleshooting sections

### Content mapping

| Source | Target section |
|---|---|
| Template class names + roles | `What This Module Generates` bullets |
| Factory extension contributions | Feature section describing what gets added/configured |
| `AppSettingRegistrationRequest` keys registered | `Module Settings` section |
| `ContainerRegistrationRequest` / `HasDependency` targets | `Related Modules` hints |
| NuGet packages introduced | Callout in relevant feature section or `External Resources` |

===

## Artifact 3: `.imodspec` Fields

Edit the `*.imodspec` file in the module directory directly. Do not edit files inside `.intent/` folders.

### Fields to set

| Field | Rule |
|---|---|
| `<summary>` | 5–15 words; name the technology and what it does. Specific, not generic. |
| `<description>` | Mirror `<summary>` exactly. Only expand (max 2 sentences) if the module is complex enough to warrant it. |
| `<tags>` | 3–7 space-separated lowercase keywords (see tag guide below) |
| `<projectUrl>` | Only set this if the maintainer supplies a URL (e.g. the module's repo or README link). Never guess or fabricate one — omit the field entirely when none is given. |
| `<releaseNotes>` | Copy `release-notes.md` content verbatim — the imodspec embeds the same history |
| `<authors>` | Your organization or publisher name — whoever maintains this module |

### Tag selection guide

- *Include:**
- Technology name: `nservicebus`, `masstransit`, `entityframework`, `serilog`, etc.
- Architectural pattern: `eventing`, `messaging`, `outbox`, `cqrs`, `ddd`, `persistence`
- Layer: `infrastructure`, `application`
- Stack: `csharp`, `dotnet`
- Transport (if applicable): `rabbitmq`, `azureservicebus`, `amazonsqs`
- *Avoid:** generic words like `module`, `common`, `helper`, `base` unless the module is genuinely a base/utility module.

### Example

```xml
<summary>NServiceBus messaging infrastructure for Intent Architect .NET applications.</summary>
<description>NServiceBus messaging infrastructure for Intent Architect .NET applications.</description>
<tags>nservicebus messaging eventing infrastructure csharp dotnet</tags>
```

===

## Verification Checklist

After completing all three artifacts:

- [ ] `release-notes.md` — entry exists for current module version (stripped); uses only `New Feature:` / `Improvement:` / `Fixed:` prefixes; reverse chronological
- [ ] `docs/README.md` — H1, opening paragraph, `What This Module Generates`, at least one feature section, `Module Settings` (if applicable), `Related Modules` all present; no placeholder text
- [ ] `.imodspec` — `<summary>` is specific (not generic); `<tags>` has 3–7 keywords; `<projectUrl>` is set only if the maintainer supplied one (never fabricated); `<releaseNotes>` matches `release-notes.md`
