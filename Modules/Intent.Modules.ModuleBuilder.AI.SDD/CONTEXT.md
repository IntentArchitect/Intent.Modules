# Context: Intent.ModuleBuilder.AI.SDD

## Purpose

Bundles two Intent Architect AI context files that describe and enforce the module-building
experience over the built-in Spec-Driven Development (SDD) skills (`/sdd-requirements`,
`/sdd-design`, `/sdd-tasks`, `/sdd-implement`, `/sdd-verify`, `/sdd-heal`):

- `sdd-build-module/SKILL.md` — the experience end to end: golden sample first (built in a
  planning session when none exists), a Golden Sample Reference as the artifact the spec derives
  from, then the normal SDD lifecycle shaped by it.
- `sdd-wave-evidence.instructions.md` — the obligations that must reach implementation sessions
  automatically: the wave evidence contract, find-the-Reference-first, and the `GOLDEN-SAMPLE:`
  marker sweep.

## Architectural Decisions

- **The golden sample comes before the spec.** A spec authored ahead of the sample can only
  predict the generated code, and its predictions get approved before anything can falsify them —
  requirements/design/tasks are read-only toward the model ("mutation starts at implementation"),
  so the sample cannot live inside them. Evidenced by the Wolverine eventing module (Aug 2026):
  ~1,200 lines of approved spec, 11 named assumptions, six design→requirements bounces in one day,
  one approved-but-unimplementable criterion, then four assumptions breaking within ~45 minutes of
  the first wave meeting the compiler. The spec phase list is hard-coded in the client and
  `advance_spec_phase` refuses jumps, so "before the spec" is the only sanctioned seam.
- **The experience ships as a skill plus an instruction file; the agent definition was retired.**
  Loading scope decided this, not taste: the experience *starts in Plan mode*, where a custom
  agent definition is never loaded (it exists only while its agent is selected, and never reaches
  dispatched sub-agents), whereas the `use_skill` tool is injected into every agent — including
  `plan` — and ACP sessions receive the solution's skill manifest in their system prompt
  (`AcpTurnExecutor.BuildEffectiveSystemPrompt`) plus always-applicable instruction files in
  `<instructions>` blocks. Retiring the agent also retired its frontmatter `tools:` list, a whole
  class of bug (it once omitted `advance_spec_phase`/`record_spec_verdict`, making phase gates
  literally uncallable). Session handoffs are **prepared prompts the developer pastes**, not
  spawned sessions: plan-mode entry is a frontend gesture (`/plan`), and `create_ai_task` proved
  not reliably available in real sessions.
- **The Golden Sample Reference is the real artifact; the sample is its evidence.** The primary
  document is the plan file itself (`intent/.plans/`), finished in Reference form; it may
  reference adjacent supporting files placed next to it, and anything not referenced from it does
  not exist. Variations are *explored* by adjusting the sample into a condition, capturing the
  snippet and verification into the Reference, then reverting — knowledge accumulates in the
  document while the codebase stays one clean skeleton. Discovery route for later sessions: the
  requirements document records the Reference's path; the instruction file tells implementing
  agents to read it before touching anything. This replaced a `GOLDEN-SAMPLE.md` dossier
  specification — one artifact, referenced from the spec, beats a second copy that can diverge.
- **The sample settles the core shape once; the module proves the variants.** Requiring the sample
  (or probes) to cover every capability the design cites turns each new requirement into more
  sample work and never terminates — a sample per transport × persistence mode × policy is not
  buildable, and generating that coverage is the module's whole job. Variants are declared in the
  design as test applications generated from the module (how this repository already proves
  transports and retry policies). The anti-hallucination residue that survives: never emit an API
  with neither a sample citation nor a source (docs / reflected signature / committed probe).
- **Interpolation from the sample is expected, not policed.** Requirements derive ideas from the
  Reference, but the developer stipulates the module's surface — including things the sample never
  touched. The control is prose, not machinery: extrapolations are flagged in plain language
  ("the sample shows RabbitMQ; SQS is inferred from documentation") and a large delta gets caught
  by a human reading that sentence at requirements or design review. This deliberately replaced an
  "empty assumption ledger" rule and an 11-criterion gate — see Superseded.
- **The pre-module delta is inventory AND a durability obligation, and its protection is
  self-retiring.** Before the module exists, the Software Factory strips the sample's hand-written
  wiring by definition — that itemised diff is the enumeration of what the module must generate
  (it becomes design Scope A), not a defect. Each such line is protected by a code-management
  directive (merge-style preferred: a missed cleanup then fails loudly as a duplicate, not
  silently as suppressed output) and tagged with a `GOLDEN-SAMPLE:` marker *inside* the protected
  region, naming why it exists. During implementation's parity wave, removing the marker and
  directive and confirming the template reproduces that exact line is the per-line parity proof; a
  directive left in place makes parity pass while proving nothing. `grep -rn "GOLDEN-SAMPLE:"`
  returning nothing is a done condition.

## Authoring Traps (Markdown file builder)

- Body horizontal rules must be `===`, never `---` — every standalone `---` toggles
  frontmatter-parsing and silently swallows sections.
- A line beginning with `**bold**` is corrupted into a broken `- *text**` list item. Keep bold off
  line-start. Verify generated output with `grep -n '^- \*[^ *]'`.
- A numbered list nested *inside* a bullet has its numbers silently stripped (observed in the
  retired agent definition's Phase 0). Top-level ordered lists survive; don't nest them.
- Multi-line frontmatter values (e.g. lists) parse to empty — inject via
  `.WithFrontMatter(fm => fm.Set(...))` after `.FromMarkdown(...)`.
- Never hand-set `contentHash`; never use Intent's `patch_file` on these raw strings (re-indents
  and flattens fences).
- **Content correctness cannot be confirmed from a successful build.** The only real check: build
  the module, install into the dogfooding application, run the Software Factory, read the
  generated files, and re-run until regeneration stages zero changes. Every trap above was
  invisible until that step.

## Build & Versioning

- `dotnet build --no-incremental` is required to repack the `.imod` when no C# changed —
  incremental builds skip the packaging step.
- A same-version rebuild is served stale by a running Intent Architect: the `.imod` version is the
  cache key (extracted under `.cache/modules/`) and loaded assemblies cannot unload. Bump `-pre.N`
  per template iteration, clear the cache folder, restart IA when a diff looks stale. The version
  guard also silently ignores lower-precedence versions.
- This module's `.csproj` `PackageReference`s must track its siblings (`AI.Skills`, `AI.Workflow`)
  — it was originally scaffolded with stale defaults that predate the `MarkdownFileBuilder` APIs.
- No changes to `Intent.Modules.Common` without explicit sign-off; both outputs route through the
  existing `AI.Context.Skills` / `AI.Context.Instructions` anchors.

## Module Interactions

- **Intent.Modules.Common** — provisions the anchors (`AiOutputAnchorsHelper`). No other dependency.
- **Intent.ModuleBuilder.AI.Skills / AI.Workflow** — siblings generating into the same `.agents/`
  tree. Complementary content: `AI.Workflow`'s workflow instructions state the general "compiling
  is not working" discipline that this module's evidence contract makes enforceable for SDD waves.

## Escalation Not Yet Taken

- **Shadowing `/sdd-implement` with a solution-local skill.** The built-in orchestrator treats wave
  reports as authoritative, which is what let fabricated reports propagate. The evidence contract
  works inside the built-in's own "no usable report → retry once" provision instead. If fabricated
  reports recur under the contract, a solution-local `.agents/skills/sdd-implement/SKILL.md`
  shadowing the built-in is the next step; rejected initially because shadows silently fork as the
  built-in evolves.

## Superseded

- **A dedicated `AI.Context.Agents` anchor in `Intent.Modules.Common`** — reverted at the
  developer's request; an unreviewed version bump to a foundational shared package. (Historical:
  from when this module shipped an agent definition.)
- **The agent definition (`build-module-sdd.agent.md`) and the gate skill (`sdd-golden-sample`).**
  Three generations in one pre-release line: (1) an agent md enforcing a Golden Sample as "Wave 0"
  inside implementation — failed in the Wolverine run because the spec above it was still theory;
  (2) the sample moved pre-spec behind an 11-criterion gate skill with tiers, tripwires, an
  assumption ledger and a dossier — machinery that generated friction faster than safety (it
  blocked requirements on variant coverage, which is infeasible and defeats the point of a
  generator); (3) the current shape — the gate collapsed to "the Reference exists" once the sample
  moved in front of the spec and Plan mode became the co-owned build vehicle. Lesson recorded: the
  heaviness was scar tissue from forcing the sample *into* SDD; once relocated, the policing
  machinery had nothing left to police.
