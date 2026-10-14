---
description: Fold a done Change into the Capabilities it belongs to — merge its requirements, traceability and lasting design decisions into each Capability, have the user review each one, then archive the Change. Use when a Change reaches done, or when the Specs panel's "Fold into capabilities" fills the chat.
short-description: Fold a done Change into its Capabilities, then archive it.
argument-hint: <change slug>
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - record_spec_traceability
  - request_spec_review
  - advance_spec_phase
  - ask_user_question
---

# SDD — Fold

A **Change** is one piece of work. Once it is done, what it established belongs in the **Capabilities**, the living specs of how each area of the system behaves now. You move it there, then archive the Change.

## 1. Read the Change

Resolve the slug with `read_spec()` if you weren't handed one; never guess it. The Change must be a `change` at `done`; otherwise stop and say why. Read its `requirements`, `design` and `traceability`. For a spec another framework owns, its own markers of what changed are your hints: OpenSpec's `## ADDED`, `## MODIFIED`, `## REMOVED` and `## RENAMED Requirements`, or the equivalent in the framework's documents.

## 2. Map each requirement to a Capability

From the `read_spec()` listing, read the `requirements` and `traceability` of each Capability whose title or description sounds related. For each of the Change's requirements, judge from its wording and its linked files and elements which Capability it belongs to, and whether it **adds** behaviour, **changes** an existing requirement there, or **removes** one. When more than one Capability fits, `ask_user_question`. When none fits, create one: `write_spec(newSlug, "requirements", …, description: …, kind: "capability")`, following **sdd-requirements** for the document's shape.

## 3. Write each Capability

Per Capability, with `write_spec` searchBlock/replaceBlock on its `requirements`:

- **Add:** a new requirement takes one past the highest id the Capability has ever used, and a new criterion one past the highest in its requirement. Ids are spent, never reused (see **sdd-requirements**).
- **Change:** reword the existing criteria in place, keeping their ids. State current behaviour only.
- **Remove:** delete the criterion or requirement; the ids after it keep their numbers.

Then re-record traceability against the Capability's ids, following **sdd-record-traceability**. A record replaces every link under its id, so pass the Capability's existing targets for that id plus the Change's; clear a removed requirement's links with `clear: true`. Merge the Change's lasting design decisions (why the code is shaped the way it is, not the plan for building it) into the Capability's `design`. A Capability you created always gets a `design`, in **sdd-design**'s as-built shape, because its verification gate needs one.

## 4. Review, then archive

Call `request_spec_review(capabilitySlug)` once per existing Capability you changed. On feedback, revise each commented or rejected block and request again. A Capability you created has no HEAD to diff against, so its approval is its phase gates instead: `advance_spec_phase(newSlug, "requirements", "design")`, then `advance_spec_phase(newSlug, "design", "verification")`. Only when every review and gate is approved, call `advance_spec_phase(slug, artifact: "requirements", toPhase: "archived")` for the Change. The Capabilities are then unverified until their own Verify runs.

## Dispatched by /sdd-run

As a `/sdd-run` sub-agent, do steps 1–3 and stop: the orchestrator raises the reviews and archives the Change. End with the report your instructions ask for, naming each Capability you changed or created.

## Report

Each Capability changed or created, the ids added, changed and removed in each, and whether the Change was archived.
