---
name: sdd-build-module
description: "Run the Intent Architect module-building SDD experience end to end: golden sample first (built in a planning session when none exists), then requirements, design, tasks and implementation derived from its Reference document. USE ONLY WHEN building a new Intent Architect module or a major feature of one, from 'where do we start' through implementation. DO NOT USE FOR ordinary application development in a consuming app, a designer-only metadata fix with no generated-code impact, or executing a single SDD wave (the sdd-wave-evidence instructions govern that)."
keywords: [module-building, sdd, golden-sample, planning, reference]
template-id: Intent.ModuleBuilder.AI.SDD.Skills.SddBuildModule_SkillMd_Agents
contentHash: 515CBE21F5A3A5AFFEE889B8F249307E04014DE344FB7E996073550732591974
---
# Skill: sdd-build-module

The module-building experience in an SDD context, end to end. One prerequisite governs everything:
a **golden sample** — at least one working variation of the thing the module will generate — and
its **Golden Sample Reference**, the document the spec phases derive from. Everything after that is
the normal SDD lifecycle, shaped by the Reference.

```
/sdd-build-module
  └─ "Do you have a reference golden sample?"
       ├─ yes → ensure the Golden Sample Reference exists (write it from the sample if it does not)
       └─ no  → hand the developer a prepared prompt for a planning session (/plan)
                plan → developer approves → build the sample together → plan ends in Reference form
                                   │
                                   ▼
   new session: /sdd-requirements with the Reference as input   ← the only gate: the Reference exists
                                   ▼
   design → tasks → implementation → verify   (normal SDD, shaped below)
```

===

## Step 1 — Route

Ask one question: "Do you have an existing codebase or sample to use as the golden reference?"

- **Yes** → locate or write the **Golden Sample Reference** (section below). If the sample exists

  but no Reference does, documenting the sample into Reference form is the remaining upfront work —
  do that, not a re-audit of the code.

- **No** → the sample gets built in a **planning session**, because the plan document is an

  artifact the developer co-owns: rendered live, editable by them mid-flight, approved explicitly.
  Session-spawning tools are not reliably available, so the mechanism is a **prepared prompt**:
  write it out and ask the developer to start a plan session (`/plan`) and paste it. The prompt
  must contain: the `/sdd-build-module` invocation; the module's goal in a sentence; what the
  sample must settle (the core shape — where configuration, contracts, the bus, handlers, the host
  seam and persistence live); what the Reference must contain when done; and the instruction that
  the plan's own final step is the SDD handover (Step 3).

A designer-only change with no generated-code impact needs none of this.

===

## Step 2 — The planning session (no sample yet)

Inside the plan session, with this skill loaded:

1. **Research first, read-only.** Current APIs and exact signatures, current package versions,

   official documentation. An API named with no source behind it goes into the plan as an open
   unknown, with the probe that will close it.

2. **`write_plan`.** The sample's scope: the applications and topology; built by scaffolding with

   released Intent modules and the Software Factory wherever they cover the shape — the sample's
   floor then matches what a real consumer gets — and hand-written only on top. What the sample
   must settle is the **developer's call**: it is a starting point that settles the core shape
   once, never a coverage matrix of every transport, mode and policy.

3. **`implement_plan`.** The developer approves, edits-and-approves, or rejects with feedback.

Then execute the approved plan together. Disciplines, each of which earned its place in a real
failure:

- Model contracts first and generate, then hand-write the target pattern on top.
- Protect every hand-written line inside an Intent-managed file the moment it is written, and tag

  it: `// GOLDEN-SAMPLE: pre-module delta — remove when the owning template generates this line.`
  The tag sits inside the protected region, or the next regeneration strips it.

- Run the Software Factory and read the staged diff. What it would strip **is the enumeration of

  what the module must generate** — record it in the Reference. Then re-run until the diff is
  clean, so the sample survives regeneration.

- Explore a variation by adjusting the sample into that condition, capture the snippet and what was

  verified into the Reference, then revert the sample to its base skeleton. The knowledge lands in
  the document; the codebase stays one clean skeleton.

- Never edit generated output to make the sample look right.

Finish by rewriting the plan document into **Reference form** and committing the sample.

===

## The Golden Sample Reference

The primary artifact is the plan document itself (in `intent/.plans/`). It may reference adjacent
supporting files placed next to it — snippets, exploration notes, a delta inventory — but anything
not referenced from the primary document does not exist. Required content:

- **Where things live** — a file map of the sample: configuration, contracts, bus, handlers, host

  seam, persistence, with a line each on what it demonstrates.

- **Key snippets** — the code a later session would otherwise have to scout the sample for.
- **Variations explored** — for each: what changed, the snippet, how it was verified (compiled,

  ran, reflected), and confirmation the sample was reverted to base.

- **Pre-module delta** — every line the Software Factory strips, each with its `GOLDEN-SAMPLE:`

  marker and protecting directive. This list is what the module must generate.

- **Out of scope** — what the sample deliberately does not show, so absence is never mistaken for

  oversight.

Once planning wraps, the Reference is authoritative: a session that needs the sample reads the
Reference first and follows its pointers into code, instead of scouting the codebase.

===

## Step 3 — Hand over to SDD

The plan's final step — written into the plan itself, because approval resets the conversation to
the plan alone — is to produce the SDD handoff prompt for the developer to paste into a **new
session**:

- the `/sdd-requirements` invocation
- the Reference's path, with the instruction to read it before any interviewing
- context worth carrying forward: what was descoped, what is extrapolated, open reservations

The gate is one sentence: **requirements do not start until the Reference exists.** And the first
act of requirements is to record the Reference's path in the requirements document, so every later
session finds it through the spec.

===

## Requirements and design, shaped by the Reference

- The Reference is **input, not authority**. Derive ideas from it, but the developer stipulates

  what the module actually needs — including variations the sample never touched.

- **Flag extrapolation plainly**: "the sample shows RabbitMQ; SQS is inferred from documentation."

  A large delta between interpolated points is caught by a human reading that sentence in review —
  at requirements, or at latest design. That is the mechanism; there is no other gate.

- A criterion that depends on an Intent platform capability cites a shipped precedent or a probe.

  This is what stops an approved-but-unimplementable requirement.

- Where the sample has no runtime tests, phrase acceptance criteria as **generated shape**, never

  behaviour — nothing may claim a message is delivered, retried, or handled.

- The design names which variations get **test applications** generated from the module. That is

  where transports, persistence modes and policies get proven — never by growing the sample.

===

## Implementation and verify

Normal SDD. Waves: metamodel → templates → parity against the sample → test applications. The
`sdd-wave-evidence` instructions bind every wave agent: evidence per ticked task, read the
Reference before implementing, and the `GOLDEN-SAMPLE:` marker sweep — removing a marker's
directive and confirming the template reproduces that exact line is the per-line parity proof.

If implementation reveals the sample itself is wrong or incomplete, fix the sample, update the
Reference, then resume. Never adjust a template to match a sample known to be wrong, and never
edit the sample to match what the templates happen to produce — the direction of correction is
what makes the sample worth having.
