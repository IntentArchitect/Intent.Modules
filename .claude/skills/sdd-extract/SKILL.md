---
description: Document a feature that already exists as a finished SDD spec in one run — scope, behaviour inventory, requirements, design, tasks, traceability and a verified verdict — orchestrating read-only sub-agents and the as-built mode of each phase skill. Use when the user asks to extract, document or back-fill a spec for existing code.
short-description: Extract a verified spec from an existing feature, end to end.
argument-hint: <feature or module>
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - advance_spec_phase
  - record_spec_traceability
  - complete_spec_task
  - record_spec_verdict
  - get_change_review
  - create_sub_agent
  - ask_user_question
  - todo_update
---

# SDD — Extract (orchestrator)

You turn **a feature that already exists** into a spec that reaches `done` in a single run: requirements, design, tasks, traceability and a verdict, all describing the code as it is.

**This is a user decision.** The user chose to document existing behaviour rather than design new work. Per `guidance-precedence`, that makes the **As-built mode (extraction)** section in each phase skill authorised. Follow it wherever it differs from that skill's interview or design rules. When you delegate, say "the user asked to extract an existing feature" so sub-agents can tell the instruction from your own inference.

**You own every write and every gate.** `advance_spec_phase` can't be called from a sub-agent, so sub-agents only read and report. You call `write_spec`, the gates, `record_spec_traceability`, `complete_spec_task` and `record_spec_verdict` yourself.

Seed a todo list with one item per step below and keep exactly one in progress.

## 1. Guard

Call `read_spec()` with no slug to list the specs. For any whose title or description sounds related, call `read_spec(slug, artifact: "traceability")` and check its file paths against the feature's folders. If another spec already traces most of the footprint, stop and `ask_user_question`: extend that spec, or create a new one beside it. When you go on, name every spec that overlaps the feature as a spec link, `[[other-slug]]`, in the Introduction, saying what the overlap is.

## 2. Scope

If the user named folders, files or model elements that bound the feature, those are the footprint. Hand them to every sub-agent below as the boundary, and don't widen the scope past them without asking.

Dispatch up to three `discovery` sub-agents with `create_sub_agent`, **in one response** so they run in parallel:

- **Footprint:** the UI entry points, folders, backend handlers and tests that make up the feature.
- **Model:** the designer elements involved. Tell it to load the `exploring-the-model` skill.
- **Boundaries:** the neighbouring features that touch this one but are out of scope.

Pick the slug yourself. Call `ask_user_question` **only** when the reports show two plausible boundaries. Otherwise, write the scope into the Introduction and Non-Goals and carry on.

## 3. Behaviour inventory

Dispatch up to five `discovery` sub-agents in one response, one per sub-area of the footprint, each told to load `exploring-the-model` for any model work. Each reports:

- **Behaviours:** each user-observable behaviour, with `file:line` evidence and the test that covers it (or "untested").
- **Model elements:** the elements involved, by id and name.
- **Suspected bugs:** behaviour that contradicts the code's evident intent. Examples: inconsistent handling between sibling cases, dead branches, `TODO`s, skipped tests.

Keep the merged inventory. It is the evidence every later step cites.

## 4. Requirements

`use_skill` **sdd-requirements** and **authoring-rich-documents**, then follow sdd-requirements in as-built mode.

- Make the **first** write `write_spec(slug, "requirements", …, gates: "auto")`. Auto gates are part of the user's decision: they review the finished spec once, not each phase.
- Then call `advance_spec_phase(slug, artifact: "requirements", toPhase: "design")`.

## 5. Design

`use_skill` **sdd-design** and follow it in as-built mode. Call `write_spec(slug, "design", …)`, then `advance_spec_phase(…, toPhase: "tasks")`.

## 6. Tasks

`use_skill` **sdd-tasks** and follow it in as-built mode. Call `write_spec(slug, "tasks", …)`, then `advance_spec_phase(…, toPhase: "implementation")`.

## 7. Traceability

`use_skill` **sdd-record-traceability** and follow it. For each task:

1. Call `record_spec_traceability` with the task's files and elements from the inventory.
2. Fix every reported `failure` and re-record until it's clean.
3. Call `complete_spec_task`.

The last tick moves the spec to `verification` by itself.

## 8. Verify and resolve suspected bugs

`use_skill` **sdd-verify** and follow it in as-built mode.

- If it finds gaps, reword each misdescribed criterion to match the gap's `actual`, using `write_spec` (searchBlock/replaceBlock). Clear the links the reword made stale with one `record_spec_traceability(slug, taskId: "*", requirementIds: [the reworded ids], restamp: true)` call, then verify again. Re-record with full targets only for a gap that changed which files or elements realise the requirement.
- Once the verdict passes, put the step 3 **suspected bugs** to the user, up to four per `ask_user_question` call. Give each bug three options:
  - **Fix it:** reword each such requirement to the intended behaviour. Then call `record_spec_verdict` **once** with a result for every requirement: a `gap` for each bug being fixed, and `pass` for the rest. A second call would replace the first. Then `use_skill` **sdd-heal**, follow it, and verify again.
  - **Document it as a known limitation:** add it to a `## Known limitations` list in the requirements, above `## Requirements`.
  - **Reword the requirement to match the current behaviour** (recommended when the behaviour is harmless).
- When every requirement passes, the clean verdict moves the spec to `done` by itself.

## 9. Report

Report briefly, and never paste an artifact. Include:

- the slug and the requirement count;
- coverage;
- how each suspected bug was resolved;
- anything still open.
