---
description: Take one spec from its current phase to done in a single chat — design, tasks, each implementation wave, then the fix loop (verify, one question for every disagreement, heal, one wording review, verify) — running every step in a sub-agent. Use when asked to run, continue, finish or fix a spec, or when the Specs panel's Design, Plan tasks, Implement, Verify or Fix fills the chat.
short-description: Take a spec from its current phase to done in one chat.
argument-hint: <spec slug>
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - advance_spec_phase
  - request_spec_review
  - create_sub_agent
  - ask_user_question
  - todo_update
---

# SDD — Run (orchestrator)

You take **one spec** from wherever it is to **done**, in this one chat. You are the **conductor**: every step runs in a sub-agent that writes to disk and returns a short report, and you keep only those reports. You read the spec only through `read_spec`, and only enough to choose the next step.

Two tools are yours alone, because sub-agents are denied them: **`advance_spec_phase`** (every phase gate) and **`request_spec_review`** (the wording review). Requirements are drafted in a hands-on chat with the user (`/sdd-requirements`), so a Run starts after them.

The planned human stops are the phase gates (they resolve by themselves under `gates: auto`), the one question about disagreements and tests for the rest of the spec, and the one wording review. Otherwise the user is asked only when the Run is stuck.

## Start

1. **Resolve the slug.** If you weren't handed an exact slug, call `read_spec` with no `slug` and match what the user named; if nothing plausibly matches, `ask_user_question` offering the closest specs. Use only ids and slugs that tool responses returned.
2. **Read the phase** from that listing and start at its section: design → **Design**, tasks → **Tasks**, implementation → **Waves**, verification or done → **Fix loop**. At requirements, stop: tell the user the requirements must be written and approved first, in a chat running `/sdd-requirements`.
3. Seed `todo_update` in **one** call with one item per step still ahead: the remaining phases, one per unticked Wave, and the fix loop. Keep exactly one in progress.

## Dispatching

Every step is one `create_sub_agent` call with `agentId: "agent"`. Its `instructions` name the skill to `use_skill`, the `slug`, and anything the step below adds, and end with: "Finish with a report of at most ten lines: what you wrote, ids you chose, and anything left open." Pass the `applicationId` the spec implements to every step that changes code.

Sub-agents call `ask_user_question` directly, so a question mid-step is theirs: wait for the report.

## Design

Dispatch `/sdd-design`. Tell it: "write design.md with write_spec and stop at the phase gate; the orchestrator raises it." Then call `advance_spec_phase(slug, artifact: "design", toPhase: "tasks")`. On reject, dispatch `/sdd-design` again with the user's feedback, then raise the gate again. Done when the spec is at **tasks**.

## Tasks

Dispatch `/sdd-tasks` with the same instruction for tasks.md. Then call `advance_spec_phase(slug, artifact: "tasks", toPhase: "implementation")`, handling a reject the same way. Done when the spec is at **implementation**.

## Waves

Dispatch each Wave **yourself**, one at a time, in ascending id order. Sub-agents nest only two levels deep, so a wave sub-agent dispatched from here keeps its own `coding` and `discovery` sub-agents.

1. `read_spec(slug, artifact: "tasks")` for the task list and its `## Task Dependency Graph` (no graph: the whole list is one Wave).
2. For each Wave:
   - All tasks ticked: mark its todo completed and move on.
   - Otherwise dispatch `/sdd-implement-wave` for this Wave's id. For a partly ticked Wave, name its open task ids and say the ticked ones stay untouched. Pass your **carry-forward notes**: names chosen, shared helpers, deferred work and gotchas from earlier Wave reports.
   - Fold the report into the notes. When a Wave returns with tasks open because the user chose to stop, defer or revise the design, **end the Run** with the summary, quoting that decision (a design revision reads "design needs revision — `/sdd-design`"). When it returned no usable report, retry it once with the failure added.
3. **Check the phase.** Ticking the last task moves the spec to **verification** by itself, so `read_spec` with no `slug` must now show it there. When it doesn't, find the Wave whose tasks are still open and dispatch it once more, naming those task ids. If any stay open, **end the Run** with a summary that leads with "blocked: tasks still open" and lists their ids. The fix loop starts only from verification: a clean verdict moves verification to done and nothing earlier.

Done when the spec is at **verification**.

## Fix loop

Tests that cite criteria are the durable proof here: a criterion whose citing tests are confirmed and current is never judged again, which is what lets the loop converge. It runs **once**, in this order, with at most one extra round (step 6).

1. **Verify** in a fresh sub-agent, adding nothing but what this step names: no reports, no notes, no gap briefs. Always add "Also run every test citing the spec once.", so a test that broke since the last Run is healed in this loop rather than found at its end.
   - **No verdict yet**, or the user asks for one: a **full** verify. Dispatch `/sdd-verify` with "Verify spec `<slug>`. Trust no earlier claim; check everything yourself." plus the report line.
   - **Otherwise** a **targeted** one. Read `read_spec(slug, artifact: "verdict")` and `read_spec(slug, artifact: "tests")`; the **ids** are the verdict's gap rows, the requirements in `movedSinceVerdict` (for a code move that lists `untestedCriteria`, just those criteria: tests prove the rest), and the criteria whose test status is `stale` or `unconfirmed`. Dispatch `/sdd-verify` with "Re-verify spec `<slug>`: judge only <ids>. Trust no earlier claim about these ids." plus the report line. Nothing to list: dispatch it anyway, with "Re-verify spec `<slug>`: judge no requirement." and the run-every-test line.
2. **Ask once.** Put both of these to the user in **one** `ask_user_question` call (pass the slug as `spec`). With neither, ask nothing.
   - **Every disagreement.** The gaps whose `reason` starts with `Disagreement:` are criteria their citing tests disagree with, and neither side wins by default. One question per disagreement, bundling the rest when there are more than four, each saying in plain words what the criterion requires and what the test and code do, with the choices **fix the code**, **change the test** and **reword the criterion**. Recommend one when the verify report makes the right side clear.
   - **Tests for the rest of the spec.** Re-read `read_spec(slug, artifact: "verdict")` and `read_spec(slug, artifact: "tests")`, and count the untested and partly tested criteria that no gap names (each gap's `expected` starts with its criterion ids). When there are any, ask whether to write their tests now, naming how many criteria and heal waves (about twenty-five criteria a wave) that is, with the choices **write them now** and **leave them**. Recommend **leave them** when it is more than one wave.
3. **Heal** when there are gaps or answers, or the user chose **write them now**: dispatch `/sdd-heal` with the answers from step 2. It reads the gaps and test statuses itself and writes tests for the criteria its gaps name; for **write them now**, also name the requirements whose untested criteria it covers. When more than about twenty-five criteria need tests, dispatch it in waves, one after another: whole requirements grouped up to about twenty-five criteria each, never one small requirement per wave, with each gap in the wave that covers its requirement. Add a todo per heal wave as you plan them (`Heal wave 2/5: R3, R4`). Hand each wave your carry-forward notes, so it doesn't map the code again: which files and test harnesses realise which requirements, helpers earlier waves added, and gotchas. Heal records its own bindings, so the Specs panel shows the tests as each wave lands.
4. **One wording review.** When heal reworded criteria, call `request_spec_review(slug)` once for all of them. Call it again only to act on comment feedback: revise each commented block, then request again, which continues that same review. A **rejected** rewording is restored, so the code and test must change instead: dispatch `/sdd-heal` once more with that disagreement answered **fix the code**.
5. **Verify once more**, targeted as in step 1: read `verdict` and `tests` again first, because heal's edits usually move other requirements that share its files, and those belong in the ids too, adding "Also run every test citing the spec once." to its instructions, and, when heal reported criteria as untestable, "Heal reported these untestable; have the sceptic judge them: <id: reason; …>". Run it even with no ids to list, since a later heal wave can break an earlier wave's tests. A criterion heal bound is `tested` and only has its tests run, unless a rewording or a later test edit made it stale.
6. **One more round, only when step 5 recorded gaps.** Run steps 2–5 once more for those gaps alone: ask about their disagreements (no tests question), heal them, review any rewording, and verify them targeted, again running every citing test. Then end with whatever that verdict says. Never a third round: put anything still open in the End summary for the user to decide.

Observations a targeted verify reports on requirements it was not asked to judge are not gaps. Do not heal them. List them in the End summary so the user can decide.

A clean verdict moves the spec to **done** by itself, whatever the gate cadence: the Run ends there, with no gate to raise.

## End

Reply with a short summary: the phases passed, the Waves run, each disagreement and how the user settled it, what the wording review changed, each verify with its gap count, test status counts and each verdict's `delta` (closed, persisting, new), the final phase, any observations from targeted verifies, and anything still open.
