---
description: Write or refresh an Area, the overview that groups related Capabilities in the Specs panel, from the Capabilities under it — goal, scope, high-level architecture and glossary — then have the user review it. Use when asked to create, group into, or update an Area, or when a new Capability needs one.
short-description: Write an Area overview from its Capabilities.
argument-hint: <area slug or name>
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - request_spec_review
  - ask_user_question
group: Spec-driven development
---

# SDD — Area

An **Area** groups related **Capabilities**. It is one overview document with no criteria, tasks, traceability or phases: it is born `done`, its health is rolled up from its children, and `advance_spec_phase` refuses it. A Capability joins an Area through `parent: <area slug>` in its manifest. Areas are one level deep, and a Change never has one.

## 1. Find the Area and its children

Call `read_spec()` with no slug. Each line names its `kind` and, for a grouped Capability, its `parent`. Reuse an existing Area when one fits. For a new one, pick a short kebab-case slug for the area of the system it covers (e.g. `ai`, `git`). When it is unclear which Capabilities belong, `ask_user_question` with the candidates.

Read each child's `requirements` and `design` (`read_spec(slug, artifact: …)`).

## 2. Write the overview

`use_skill` **authoring-rich-documents**, then write it as the Area's `requirements` artifact:

`write_spec(areaSlug, "requirements", <overview>, title: <name>, description: <one line>, kind: "area")`

Keep it short. Use these sections:

- **Goal**: what this part of the system is for, in two or three sentences.
- **Scope**: one line per child, linked as `[[child-slug]]`, saying what it covers. Say what belongs elsewhere.
- **Architecture**: how the children fit together, as one mermaid `Diagram` when it helps. Stay at a high level and leave the detail to each child's design.
- **Glossary**: only terms shared across the children.

Write no EARS criteria. Behaviour belongs in the Capabilities.

## 3. Attach the children

For each Capability that belongs and whose `parent` is not already this Area, call `write_spec(childSlug, "requirements", parent: areaSlug)` with no contents. This changes only its manifest.

## 4. Review

Call `request_spec_review(areaSlug)`. If the user leaves feedback, revise and request the review again. Do not call `advance_spec_phase`, because an Area has no phases.

## Report

The Area's slug, the children you attached, and anything still open.
