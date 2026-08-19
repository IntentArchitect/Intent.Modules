---
name: add-module-skill-template
description: "Build and maintain a module whose purpose is to distribute skill files (skills, instructions, or an agent definition) to a consuming solution through Intent Architect's normal code-generation pipeline, instead of hand-copying files into a repo. Covers the shared plumbing (Folder+FileTemplate shape, MarkdownFileBuilder, Mode.Ignore, Role/anchor routing), scaffolding a brand-new module, and the ongoing lifecycle of editing, renaming, or removing already-shipped files. Use before creating such a module, adding a new file to distribute, or changing/removing one already shipped."
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
6. **Where the generated file lands in the consuming solution is a `Template Settings.Role`, not a
   hardcoded path.** A shared anchor-resolution helper resolves `Role` to a real folder in the
   consumer's repo. Three anchors exist today — pick the narrowest one that fits:
   - `AI.Context.Skills` → `.agents/skills/<Default Location>/SKILL.md` — use for a skill file.
   - `AI.Context.Instructions` → `.agents/instructions/<Default Location>` — use for a standing
     instruction/guideline file.
   - `AI.Context` (the base anchor, `.agents/` itself) → `.agents/<Default Location>/...` — use
     for anything that doesn't fit either specific role (e.g. an agent definition would set
     `Default Location=agents` to land at `.agents/agents/...`).
   - **Never add a new anchor role to shared/common infrastructure without the developer's
     explicit sign-off** — it's a version bump to a foundational package every module depends on.
     Route through the existing base anchor with a custom `Default Location` instead.
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

## Creating a New Distributed File

Whether it's a whole new module or one more file distributed from an existing one:

- [ ] (New module only) Package references pinned to a version that actually supports the
      `MarkdownFileBuilder` / `MarkdownBaseTemplate` APIs — a freshly scaffolded module can default
      to stale versions that predate them and won't compile until bumped.
- [ ] Folder + File Template created, `File Settings(Output File Content=Text, Templating Method=
      Markdown File Builder)`.
- [ ] `Template Settings(Source=Lookup Type, Role=<see mechanism step 6>, Default Location=<name>)`
      set explicitly — a freshly scaffolded File Template defaults to different (wrong) values here;
      fix before authoring content.
- [ ] `MarkdownFile` constructor's output-path arguments left at their defaults — the real output
      path comes from `Default Location` above, not these arguments.
- [ ] After the first apply, the Software Factory may rename the File Template element (e.g.
      inserting an underscore) — treat the post-apply name as canonical; don't fight it by renaming
      back.
- [ ] Content authored directly in the constructor as a raw string, with `Body = Mode.Ignore`.

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
      change it and `Template Settings.Default Location` together, deliberately, since other
      files/tooling may reference the old name.
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
| **Single hand-authored file, no model** | One bespoke artifact with no per-consumer variation and no natural fit under a more specific role — route through the base anchor with a custom `Default Location`. |
