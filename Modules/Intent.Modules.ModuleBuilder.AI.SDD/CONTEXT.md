# Context: Intent.ModuleBuilder.AI.SDD

## Purpose

Bundles a single Intent Architect custom agent definition (`build-module-sdd.agent.md`), selectable
from Intent Architect's agent picker, that steers Intent Architect's built-in Spec-Driven Development
(SDD) skills (`/sdd-requirements`, `/sdd-design`, `/sdd-tasks`, `/sdd-implement`, `/sdd-verify`,
`/sdd-heal`) toward building/modifying Intent Architect modules specifically. It covers both sides of
that work — the module's own metamodel/templates, and the sample application used to build and
validate them — via a Golden Sample (Reference Architecture) discipline: a mandatory Wave 0 spike
that builds or updates the sample application before any metamodel/template work starts, and a
metamodel → templates → dogfood wave structure that keeps the module's generated output verified
against that sample.

## Architectural Decisions

- **The agent file's `Role` is the base `AI.Context` anchor (`.agents`), with `Default Location` set
  to `agents`** — landing the file at `.agents/agents/build-module-sdd.agent.md`. This deliberately
  reuses the anchor `Intent.Modules.Common` already provisions in every consuming application, rather
  than adding a new `AI.Context.Agents` anchor to that shared package. A dedicated anchor was tried
  first and reverted — see "Superseded" below — because it required a version bump to a foundational
  package every other module depends on, which is out of scope for a single new module's needs.
- **The File Template element name ends up as `BuildModuleSdd_AgentMd`, not the
  `BuildModuleSddAgentMd` it was created with.** Intent Architect's Software Factory renamed it
  (inserting the underscore) the first time `apply_staged_file_changes` ran. This looks like an
  engine-side naming convention tied to the `AI.Context` family in general (mirroring
  `SkillMd_Agents` in `AI.Skills`/`AI.Workflow`) rather than anything specific to a particular Role
  value — it also applied when the Role was still `AI.Context.Agents`, and persisted after switching
  to the base `AI.Context` anchor. Do not fight it by renaming back; treat the post-apply name as
  canonical and keep the model and generated folder/class names in agreement.
- **The bundled content is verbatim, hand-authored into the generated `TemplatePartial.cs`'s
  constructor body** (`Body = Mode.Ignore`, so the Software Factory never touches it again), exactly
  like `AI.Workflow`'s `ModuleBuildingWorkflowMdTemplatePartial.cs`. There is no per-consumer
  parameterization — every installing application gets the identical agent definition.
- **Body-level horizontal rules use `===`, not `---`.** `Intent.Modules.Common`'s
  `MarkdownFileParser.ParseFrontMatter` treats *every* standalone `---` line as a frontmatter
  delimiter, not just the first pair — additional `---` lines used as section separators in the body
  toggle it in and out of "frontmatter mode" and silently swallow whole sections. Only the two real
  frontmatter delimiters (first and last) may be `---`; any other horizontal rule in the body must be
  `===` (or another non-`---` marker) or content between them gets dropped on generation.
- **The `tools:` YAML list is injected via `.WithFrontMatter(fm => fm.Set("tools", ...))` after
  `.FromMarkdown(...)`, not left inline in the raw markdown.** `MarkdownFrontMatter`'s parser only
  understands single-line `key: value` pairs — a multi-line YAML block list (`tools:` followed by
  `- item` lines) parses as `tools: ""` and every item line is silently discarded (no error). The
  workaround: let `.FromMarkdown(...)` parse the frontmatter as usual (the inline `tools:` block in
  the raw content is effectively decorative at that point), then overwrite the `tools` entry
  afterwards with the correctly-formed multi-line value via `Set`, exploiting the fact that
  `MarkdownFrontMatter.ToString()` naively concatenates `$"{key}: {value}"` for each entry — a
  `value` containing embedded `\n  - item` lines round-trips correctly even though the parser can't
  read it back in. This is a workaround for a real limitation in `Intent.Modules.Common`'s Markdown
  file builder (it does not support list-valued frontmatter properties at all), fixed here rather
  than in that shared package per the same "no changes to `Intent.Modules.Common` without sign-off"
  constraint above. If another module ever needs a list-valued frontmatter property, copy this
  pattern rather than re-discovering it.

## Invariants & Constraints

- This module's own `.csproj` `PackageReference`s (`Intent.Modules.Common`, `Intent.SoftwareFactory.SDK`)
  must track the same versions its siblings (`AI.Skills`, `AI.Workflow`) use — the module was
  originally scaffolded with stale defaults (`Intent.Modules.Common` 3.7.2) that predate the
  `MarkdownFileBuilder`/`MarkdownBaseTemplate` APIs this module's template depends on, and would not
  compile until bumped to match.
- This module makes **no changes to `Intent.Modules.Common`** and depends only on the `AI.Context`
  anchor that package has always provisioned. Do not reach for a new dedicated anchor there without
  the developer's explicit sign-off first — that is a change to shared, foundational infrastructure
  every module depends on, not a decision scoped to this module alone.

## Module Interactions

- **Intent.Modules.Common** — provisions the base `AI.Context` anchor (`.agents`) this module's
  template routes through, via `AiOutputAnchorsHelper`. No other dependency.
- **Intent.ModuleBuilder.AI.Skills / Intent.ModuleBuilder.AI.Workflow** — sibling modules in the same
  `AI.*` family, generating skills/instructions into the same `.agents/` tree via the same base
  anchor mechanism (their own `Role` values point at the more specific `AI.Context.Instructions` /
  `AI.Context.Skills` anchors those two modules were already built against). No package dependency
  between them (each installs independently).

## Superseded

- **A dedicated `AI.Context.Agents` anchor in `Intent.Modules.Common`.** First attempt: extended
  `AiOutputAnchorsHelper` with a third anchor mirroring Instructions/Skills, bumped
  `Intent.Modules.Common` to `3.11.6-pre.0`, added a version migration. Reverted at the developer's
  explicit request — an unreviewed version bump to a foundational shared package was not something
  they'd asked for. Replaced with the `Role=AI.Context` + `Default Location=agents` approach above,
  which needs no change outside this module.
