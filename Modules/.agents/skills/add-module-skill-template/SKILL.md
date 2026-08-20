---
name: add-module-skill-template
description: "Set up the MD template (Folder+File Template, MarkdownFileBuilder, frontmatter) for a new instruction or skill file to be generated and distributed by an Intent Architect module, or maintain one already shipped this way. USE ONLY WHEN adding, editing, renaming, or removing an instruction/skill file distributed by a ModuleBuilder.AI.* module. DO NOT USE FOR any other skill implementation — only for skills/instructions generated via an Intent module's code-generation pipeline, never one authored directly in a consumer's .agents folder, and never for an unrelated module change such as a version bump (see module-versioning). REQUIRES the target ModuleBuilder.AI.* module's Module Builder designer already open."
keywords: [skill-files, code-generation, module-builder, distribution, anchors, mode-ignore, frontmatter]
---
# Skill: add-module-skill-template

Some modules generate no application code at all — their entire purpose is to **distribute skill
files** (skills, instructions, an agent definition) to a consuming solution, using Intent
Architect's ordinary code-generation pipeline as the delivery mechanism instead of hand-copying
files into a repo. This skill covers that mechanism end to end: standing up a new distributing
module, adding a new file to an existing one, and maintaining files that are already shipped.

## When to Use

- Creating a new module whose purpose is to distribute skill files via code generation.
- Adding a new file to distribute from an existing module.
- **Editing the content of an already-shipped file.**
- **Removing or deprecating a distributed file.**

## The Shared Mechanism

1. **Modeled as an element, not a file.** One `Folder` containing one `File Template` element
   (type `Single File`) per file to be distributed.
2. **Each File Template generates two classes**: a `...TemplateRegistration.cs` (pure boilerplate,
   always fully generated — never hand-touch it) and a `...TemplatePartial.cs` (the actual
   content).
3. **The Partial builds a `MarkdownFile`** — extends `MarkdownBaseTemplate<object>`, implements
   `IMarkdownFileBuilderTemplate`, and calls `.FromMarkdown(...)` with the frontmatter + body as one
   raw string literal.
4. **The constructor is `Mode.Fully, Body = Mode.Ignore`.** This is what protects hand-authored
   content from being clobbered by later Software Factory runs — it's treated as source, not
   scaffold, the same way a hand-written method body would be. It's also why you can go edit that
   raw string directly later — nothing regenerates over your change.
5. **A `contentHash` in the frontmatter is how Intent decides whether to push an update into an
   already-installed file.** Setting `WithContentHashing = true` on the template makes every
   generation: read the consumer's existing file on disk, recompute a SHA-256 hash of its content
   (the hash field itself excluded from the computation), and compare that to the hash already
   stored in the file.
   - **Hashes match** → the installed file is byte-identical to what was last generated, so it's
     safe to overwrite with the module's latest content, and a fresh hash is stamped into the
     regenerated file.
   - **Hashes don't match** → the consumer edited the file by hand since it was installed, so the
     generator leaves it alone rather than clobbering their edit.
   - This is a *different* protection from `Mode.Ignore` above: `Mode.Ignore` protects the
     module's own source (the `TemplatePartial.cs` raw string in this repo) from the Module
     Builder's own scaffolding; the content hash protects the *consumer's installed copy* from
     being silently overwritten by a routine module update. Never hand-set the `contentHash` value
     yourself — it's only ever meaningful as something the mechanism recomputes.
6. **Where the generated file lands in the consuming solution is a `Template Settings.Role` plus the
   `MarkdownFile` constructor's `relativeLocation` argument — never the `Default Location` template
   setting.** A shared anchor-resolution helper resolves `Role` to a real folder in the consumer's
   repo, and `relativeLocation` (the constructor's second argument, e.g. `new MarkdownFile("SKILL",
   relativeLocation: "add-module-skill-template")`) is appended to it. `Default Location` only ever
   seeds that argument's value at the moment the File Template element is first created — after that
   the constructor's `Body = Mode.Ignore` means the model setting is never read again, so leaving it
   set is misleading (it looks live but does nothing). Leave it unset and bake the path straight into
   the constructor instead. Three anchors exist today — pick the narrowest one that fits:
   - `AI.Context.Skills` → `.agents/skills/<relativeLocation>/SKILL.md` — use for a skill file.
   - `AI.Context.Instructions` → `.agents/instructions/<relativeLocation>` — use for a standing
     instruction/guideline file.
   - `AI.Context` (the base anchor, `.agents/` itself) → `.agents/<relativeLocation>/...` — use
     for anything that doesn't fit either specific role (e.g. an agent definition would pass
     `relativeLocation: "agents"` to land at `.agents/agents/...`).
   - **Never add a new anchor role to shared/common infrastructure without the developer's
     explicit sign-off** — it's a version bump to a foundational package every module depends on.
     Route through the existing base anchor with a custom `relativeLocation` instead.
7. **Two silent-failure frontmatter traps — they bite on a one-line edit just as easily as on a
   brand-new file:**
   - The parser only understands flat `key: value` lines. A multi-line YAML list (e.g. `tools:`
     followed by `- item` lines) parses as `tools: ""` and **every item is silently dropped, no
     error**. Fix: let `.FromMarkdown(...)` parse as usual, then overwrite afterward with
     `.WithFrontMatter(fm => fm.Set("tools", "...\n  - item1\n  - item2"))` — the naive
     `ToString()` round-trips a value containing embedded `\n  - item` lines correctly even though
     the parser can't read that shape back in.
   - Every standalone `---` line toggles frontmatter-parsing mode on/off. Only the first and last
     `---` may be real frontmatter delimiters — any other horizontal rule in the body **must** use
     `===` (or another non-`---` marker), or the content between them is silently dropped.

## Writing the Skill's `description` Frontmatter

When the file being distributed is itself a **skill** (`Role=AI.Context.Skills`), its `description`
is a **semantic router**, not a summary: it's the *only* thing a consuming AI harness reads before
deciding whether to load the skill at all — the body isn't consulted until that decision is already
made. Its job is to prevent false-positive invocations, not to read well. Get it right before
shipping — once a consumer has adopted the skill on the strength of its description, tightening it
later is a version bump, not a free edit.

Every `description` **must** contain these four parts, in order, and nothing else:

- **Capability** — an objective, third-person statement of the exact action the skill performs.
  Not a category label ("Skill for module distribution") — the actual deliverable.
- **`USE ONLY WHEN`** — the precise triggers, requests, or contexts that call for this skill.
- **`DO NOT USE FOR`** — the adjacent or easily-confused tasks that must bypass it instead (e.g.
  distinguishing read-only analysis from a mutation, or a broad question from this specific
  workflow). Name the real neighboring skill/tool if one exists.
- **`REQUIRES`** — mandatory inputs, context, or tools that must already be in place before this
  skill runs. Omit this part entirely if there is genuinely no prerequisite — don't write a
  placeholder just to keep the shape.

Forbidden, no exceptions: first-person phrasing ("I can help you..."), vague verbs ("assists
with", "supercharges", "handles"), and open-ended catch-alls. Write only what changes the routing
decision — nothing added for polish, and nothing restating the skill's name or body.

**Parser constraint — this determines the physical shape, read before drafting.** Mechanism step 7
above already documents that this project's frontmatter parser only understands flat, single-line
`key: value` pairs; anything spanning multiple physical lines (a YAML list, or a `>`/`|` block
scalar) silently parses to an empty value with no error. So the four parts go on **one physical
line inside a single quoted string** — not the multi-line block-scalar shape a generic YAML
contract might suggest:

```yaml
description: "[Capability, 3rd-person]. USE ONLY WHEN [explicit triggers]. DO NOT USE FOR [adjacent/out-of-scope tasks]. REQUIRES [mandatory inputs — omit this sentence if there are none]."
```

If a distributed file's frontmatter genuinely needs a multi-line value, don't hand-write it into
the raw string — apply the same workaround as the `tools:` list trap: let `.FromMarkdown(...)`
parse normally, then overwrite with `.WithFrontMatter(fm => fm.Set("description", "...\n..."))`
after the fact.

Good vs. bad, for a skill that adds validation rules to a designer element:

- Bad: `"Assists with validation stuff for modules."` — vague verb, no triggers, no boundary.
- Good: `"Add or change a validation rule attached to a Module Builder element so it surfaces as a
  designer error before code generation. USE ONLY WHEN a property or association needs a required,
  uniqueness, or format check enforced at design time. DO NOT USE FOR runtime/business-rule
  validation in generated application code — that's modelled separately per target framework.
  REQUIRES the target element to already exist in the designer."`

## Creating a New Distributed File

Whether it's a whole new module or one more file distributed from an existing one:

- [ ] (New module only) Package references pinned to a version that actually supports the
      `MarkdownFileBuilder` / `MarkdownBaseTemplate` APIs — a freshly scaffolded module can default
      to stale versions that predate them and won't compile until bumped.
- [ ] Folder + File Template created, `File Settings(Output File Content=Text, Templating Method=
      Markdown File Builder)`.
- [ ] `Template Settings(Source=Lookup Type, Role=<see mechanism step 6>)` set explicitly — a
      freshly scaffolded File Template defaults to a different (wrong) `Role` here; fix before
      authoring content. Leave `Default Location` unset — it has no effect on the generated output
      path (see mechanism step 6).
- [ ] `MarkdownFile` constructor's `relativeLocation` argument set explicitly to the target path
      (e.g. `relativeLocation: "add-module-skill-template"`) — this is the real output path, not
      the model setting.
- [ ] After the first apply, the Software Factory may rename the File Template element (e.g.
      inserting an underscore) — treat the post-apply name as canonical; don't fight it by renaming
      back.
- [ ] Content authored directly in the constructor as a raw string, with `Body = Mode.Ignore`.
- [ ] **If distributing a skill** (`Role=AI.Context.Skills`): its `description` written to the
      four-part contract in [Writing the Skill's `description` Frontmatter](#writing-the-skills-description-frontmatter)
      (Capability / `USE ONLY WHEN` / `DO NOT USE FOR` / `REQUIRES`) — checked against the good/bad
      example there, not just drafted and left.

## Maintaining an Already-Shipped File

Once a file has been generated at least once, most changes to its *content* need no model change
at all:

- [ ] Locate the specific `...TemplatePartial.cs` — not `...TemplateRegistration.cs`, which is
      pure boilerplate and never carries content.
- [ ] Edit the raw string in place. The two frontmatter traps above apply just as much here.
- [ ] **Never hand-edit the `contentHash`** in the frontmatter — it's recomputed automatically on
      every generation (see mechanism step 5) and is what tells Intent whether a consumer's copy is
      still untouched (safe to overwrite) or was hand-edited (leave alone). Setting it yourself
      breaks that check in one direction or the other.
- [ ] **Renaming** the file (its frontmatter `name:`) is an identity change, not a wording tweak —
      change it and the constructor's `relativeLocation` argument together, deliberately, since
      other files/tooling may reference the old name.
- [ ] **Removing** a file means deleting the Folder+File Template model element. The next Software
      Factory run on a consumer will propose **deleting** the previously generated file — this
      surfaces as a destructive change; confirm it's intended before applying, don't wave it
      through.
- [ ] Re-ran the Software Factory against an application that consumes this module and checked
      `get_file_diffs` shows exactly the intended change before `apply_staged_file_changes`.

### Every change ships on its own schedule

A distributed file only reaches a consumer when they update to a new package version of the
module — there's no other delivery path. Treat every edit to already-shipped content as a real
release, not just a markdown tweak: bump the module's version and note the change in its release
notes, even for a wording-only fix — without that, installed consumers never see the update.

## Shape Picker

| Shape | Choose when |
|---|---|
| **Unconditional file** | The content is a reference/mechanics doc — always identical for every installer, no settings involved. |
| **Settings-driven file** | The content's presence or wording depends on a module setting (e.g. a boolean) — read the setting in the template and adapt what's emitted. |
| **Single hand-authored file, no model** | One bespoke artifact with no per-consumer variation and no natural fit under a more specific role — route through the base anchor with a custom `relativeLocation` argument in the constructor. |
