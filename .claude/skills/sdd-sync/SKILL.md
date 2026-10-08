---
description: Keep specs true after code changes — find the requirements whose traced files or elements changed, judge whether each still holds, write the updated requirements and have the user review them in the diff with request_spec_review. Load before finishing any turn that changed code a spec traces, or when a tool result carries a "Spec:" hint.
short-description: Update specs whose traced code just changed, reviewed in the diff.
argument-hint: Optional spec slug
intent-updates: automatic
requiredTools:
  - get_change_review
  - get_file_diffs
  - read_spec
  - write_spec
  - request_spec_review
  - record_spec_traceability
  - complete_spec_task
  - ask_user_question
  - create_sub_agent
---

# SDD — Sync

Code that a spec traces has changed. Your job is to make sure the spec still describes it. **Every change to a spec's requirements goes through the user's review in `request_spec_review`.**

**Skip this skill** when this conversation is running `sdd-implement`, `sdd-implement-wave`, `sdd-heal` or `sdd-extract` for the same spec, because those flows already own its traceability. Also skip it in `sdd-verify` and `sdd-trace-reconcile`: they review and link, and must not reword requirements. The one exception is a **Run**: when `sdd-run` loads this skill as a step of its fix loop, run it, whatever else that Run has done for the spec.

## 1. Find what's affected

A shortcut for steps 3–4: for each spec, `get_change_review(section: "files", refA: "HEAD", specSlug: <slug>)` lists each uncommitted file with the requirements linked to it, including that spec's links recorded before the change, and `section: "elements"` does the same for model elements. It shows neither deleted elements nor output the Software Factory staged but didn't write, so step 2 still applies.

1. **Ask each spec what moved since it was verified.** For each spec `read_spec()` lists, call `read_spec(slug, artifact: "verdict")`. Its `movedSinceVerdict` names each requirement whose traced code changed, whose wording changed, or both, with the files and elements that changed. A code move under criteria that are all tested isn't listed, because their tests prove it; for the rest it names the `untestedCriteria` to judge. It compares content, so it also catches changes made outside this conversation, such as a teammate's commit or a merge.
2. **List what changed in this conversation.** Changed files come from `git status --short` (on disk) and `get_file_diffs` (staged by the Software Factory). Changed model elements, with their names, come from `get_change_review(section: "elements", refA: "HEAD")`.
3. **List what the specs trace.** Call `read_spec()` for the specs. For each one, call `read_spec(slug, artifact: "traceability")`: each requirement's compact targets carry the file's `relativePath` or the element's name.
4. **Intersect them.** An affected requirement is one `movedSinceVerdict` lists, or one whose target is among this conversation's changes. Every `Spec:` line in this conversation's write, patch or delete results already names one, so start from those.
5. **Look for candidate new behaviour:** changed or added files that no requirement traces, sitting in the same folders as a spec's traced files. Only files next to traced ones count, so this doesn't turn into a whole-repo review.

If nothing traced changed and there are no candidates, say so in one line and stop.

## 2. Judge each requirement

For each affected requirement, read its statement with `read_spec(slug, artifact: "catalog", requirementId: …)`, its links with `read_spec(slug, artifact: "traceability", requirementId: …)`, and its section with `read_spec(slug, artifact: "design")`. Read the change with `get_file_diffs` for staged Software Factory changes, or `git diff` for changes already on disk. For a change made before this conversation, read the changed targets as they are now. Classify it as one of:

- **still true:** the change doesn't alter what the criteria say;
- **now false:** a criterion no longer describes the behaviour;
- **extended:** new behaviour that no criterion covers yet;
- **removed:** a linked file or element was deleted, so the link is broken.

Two cases from `movedSinceVerdict` skip the classification. If only the **wording** moved, the requirement was reworded after it was verified: don't judge it here, but include it in the step 5 verify. If **both** moved, ask the user which one is right.

With more than about six requirements, dispatch one `discovery` sub-agent per spec, in one response. Each should return a classification plus `file:line` evidence per requirement.

## 3. Write the changes and get them reviewed

**Still true** needs no change. For each requirement that is **now false**, **extended** or **removed**:

1. Say in one line which criteria you are changing, e.g. "This changes ai-chat-surface R7.3 and adds R12.18."
2. Patch `requirements` with `write_spec` using searchBlock/replaceBlock so the criteria describe the code. State current behaviour only. For new behaviour, first choose the spec: list them with `read_spec()` and put it in the one whose description covers it, whichever spec traces the code, asking the user when more than one fits. Then add a new `### Requirement N` numbered one past the highest ever used. For behaviour that is gone, delete the criterion and leave the ones after it numbered as they are: ids are spent, never renumbered or reused, because tests cite them (see **sdd-requirements**).
3. Call `request_spec_review(slug)` once for the spec, after every write. The user reviews each changed block in the diff against git HEAD and accepts, rejects or comments on it. Approval restamps the reworded requirements' links, so don't restamp them yourself.
4. If the review comes back with feedback, act on each item. For a **rejected** block, restore its original wording and change the code back so that wording holds again. For a **comment**, revise the wording as asked. Then `write_spec` and `request_spec_review` again. Blocks the user already accepted stay accepted. In a Run, restore the rejected wording and hand each rejected block back to `sdd-run` as a gap instead of changing the code yourself: its heal step makes the code match the wording.

5. **Re-check the tests of every reworded criterion, in this same turn.** `read_spec(slug, artifact: "tests", requirementId: <its requirement>)` lists the tests citing it (titles carrying `[spec:<slug> R7.3]`). When the user chose to change a test, update it first and run it. Then have one `discovery` sub-agent judge each reworded criterion against its citing tests as a sceptic: which of its clauses they assert (`full`, `partial` with what is missing, or `mismatch`). Report the result: a `mismatch` is a disagreement for the user, and the next verify records the new degree as a binding.

`request_spec_review` refuses a spec another framework owns. For that spec, ask about each requirement with `ask_user_question` instead (update the spec, change the code back, or leave it), and apply the answers.

## 4. Follow the approved wording

- **Design:** patch the affected realization-table rows, diagrams and `<FileRef>`s.
- **Tasks:** for a new requirement, add a task to `tasks.md` with `(satisfies: RN)`.
- **Traceability:** where targets were added or deleted, re-record that task with its existing targets **plus** the new ones and **minus** the deleted ones, because a call with targets replaces the task's links (see **sdd-record-traceability**). Call `complete_spec_task` for any new task.

This is bookkeeping that follows from wording the user approved. Do it without asking, and give it one line in the report.

## 5. Verify and hand off

Once the review is approved, or straight away when no wording needed changing, dispatch `sdd-verify` with `create_sub_agent`, targeted at the requirements `movedSinceVerdict` lists for the spec (re-read it after step 4), so the spec is left verified. Don't record a verdict yourself: a review from the session that made the change isn't independent, and the fresh sub-agent is. Don't heal what it finds: report each gap.

Report which requirements changed and how, then the verify's outcome. In a Run, skip the verify and return the summary to `sdd-run`: it verifies in its own fresh sub-agent.
