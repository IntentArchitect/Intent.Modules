---
applyTo: '**'
description: "What a wave of an Intent Architect SDD spec must produce as evidence before any of its tasks may be reported complete, including the golden-sample parity obligations that outlive the session which created them."
keywords: [sdd, wave, evidence, traceability, completion, verification, golden-sample, parity]
template-id: Intent.ModuleBuilder.AI.SDD.RootPrinciples.SddWaveEvidenceMd
contentHash: 9E40B9F5A0E734661EB4CEC4FD79D8FCB87D8AE7BE442F3605C1BBDB3C908A2B
---
# SDD Wave Evidence Contract

Scope — read the next sentence and stop if it does not apply. This applies only when you are
implementing, or reporting on, a wave of a Spec-Driven Development spec in this solution. For any
other work, ignore it.

## Why This Exists

An SDD orchestrator dispatches one sub-agent per wave and treats what comes back as authoritative —
it does not re-do the wave's work to check it. That trust is what makes wave orchestration
affordable, and it is only safe if a completion report is backed by evidence that could not exist
unless the work happened.

Two failures make this concrete, and both have happened:

- A wave reported every task complete on "both builds green". The build was green. The application

  never invoked the framework the wave existed to wire up. Green proved the syntax, and nothing else.

- A sub-agent returned a status placeholder describing what it *would* do, phrased as a report of

  what it *had* done. Three consecutive dispatches did the same. Each looked like progress.

Neither was caught by the report's tone; both would have been caught by its evidence.

## The Contract

For **every task you tick**, the report must carry:

- **The files** created or modified, by path, relative to the application root.
- **The model changes**, by element name and type, for anything modelled rather than written.
- **The command and its result** — the exact build or test command run, and the tail of its output.

  Name what was proven, not that something passed.

- **Traceability confirmation** — that the traceability record was accepted with zero failures.

A report that omits any of these, for any ticked task, **is not a completion statement**. It is an
unfinished wave, and the honest thing to return.

## Rules That Follow From It

- **Never report a task done that you did not do.** If you are blocked, say so and stop, or ask.

  A placeholder describing intended work, phrased as completed work, is the single most expensive
  thing you can return — the next wave builds on it.

- **A green build is not a working feature.** Compilation proves syntax. If a task claims behaviour,

  something must have executed that behaviour — through the application's real startup path, not a
  harness assembled inside the test that bypasses it.

- **Inspect what regeneration produced; never edit generated output to make it look right.** That

  inverts the test: it stops checking whether the template is correct and starts checking whether
  the disk matches itself.

- **An unapplied destructive diff is a defect, not a chore.** If a regeneration would strip

  hand-written code, resolve it inside the wave that created it — protect the code with a
  code-management directive, or model it properly. Leaving it for later leaves a live fault in the
  tree, and the loss lands on whoever runs the generator next.

- **Verification that matters gets its own task and its own artifact.** Where a wave is a gate,

  make its check a task whose deliverable is a file on disk — a parity or evidence report — so
  verification is durable and reviewable rather than a sentence in a transcript.

## Before You Implement: Find The Golden Sample Reference

A module-building spec is derived from a **golden sample** — a committed reference codebase the
generated output is supposed to reproduce — and its **Golden Sample Reference**, the document that
describes it. Both were very likely produced in a different session from the one you are in now,
so nothing in your context mentions them unless you go looking.

The requirements document records the Reference's path — that is the intended discovery route. If
it does not, look for the newest module-building plan in `intent/.plans/` and at the sample's
root. Read the Reference before implementing; it carries what you cannot infer from the spec
alone:

- **where things live** — which sample file each template is answerable for
- **key snippets** — the exact code your templates must emit, without re-scouting the sample
- the **pre-module delta** — the enumeration of lines the module is supposed to generate
- **variations explored** and what is **out of scope** — and therefore what the spec may not assert

If a task's wording and the Reference disagree, say so rather than picking one. The Reference
records what was actually built and verified; the spec records what someone believed at authoring
time.

## The Golden-Sample Marker Sweep

While the module did not exist, the sample's hand-written lines were held in place by
code-management directives so regeneration could not silently delete them. Each one carries a
marker with the token `GOLDEN-SAMPLE:` naming the template that will take the line over.

Those directives are **temporary scaffolding, and leaving one in place makes a parity check pass
while proving nothing** — the hand-written line stays, the template's output is suppressed, and the
diff comes back clean. So for every marker inside your wave's scope:

1. Remove the marker and its directive.
2. Regenerate.
3. Confirm the template emits that exact line. If the line disappears once unprotected, the

   template is incomplete — that is the finding, not a cleanup detail.

Step 3 is the per-line parity proof, which is why the removal is real work rather than tidying.

Sweep evidence *is* completion evidence: a parity or verification task is not complete until
`grep -rn "GOLDEN-SAMPLE:"` over the sample returns nothing, and the report says so. A marker must
never reach a real consumer: templates do not emit it, so a leftover also shows up as a parity diff.

## For The Orchestrator

Classify each wave report before acting on it. A report that violates this contract, or whose prose
disagrees with the spec's own recorded task state, is **not a usable report** — re-dispatch the wave
and name the specific deficiency. Trust in this order:

1. tool-verified state (recorded task completion and traceability)
2. evidence links (paths, commands, output)
3. prose

Reading the spec's recorded state to make that judgement is not re-verifying the wave's work; it is
checking that a report is a report.
