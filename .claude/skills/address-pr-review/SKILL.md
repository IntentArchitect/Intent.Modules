---
description: Work through a pull request's unresolved review comments — fix what should be fixed, commit them as one commit, and stage a reply per thread into the pending review. Use when asked to address, action or respond to review feedback on a pull request.
short-description: Work through a pull request's unresolved review comments — fix, commit once, and reply per thread.
argument-hint: The pull request — a number, the path identifying its repository, and the head SHA to work at
requiredTools:
  - get_pull_request_review_threads
  - get_change_review
  - read_file
  - patch_file
  - stage_pull_request_review
  - bash
---

# Address a pull request's review

Answer **every** unresolved comment on the pull request — by changing the code, or by saying plainly why you did not. The value here is that the author comes back to a branch where each thread has been dealt with and each reply names what dealt with it.

$ARGUMENTS

## The contract, up front

Three rules bound everything below. Break one and the run is worse than not having happened:

- **One commit, never a push.** All your work lands in a single commit at the end (§5); the user pushes it from the pull request's Conversation tab. Never `git push`, never rewrite history (no `commit --amend`, no `rebase`, no `reset --hard`), never touch a branch other than the one you are on.
- **Reply to every thread you looked at**, including the ones you decided not to change. A thread with no reply reads as a thread you missed.
- **Nothing reaches the git host.** `stage_pull_request_review` puts your replies in the user's own PENDING review, on their machine. They read them, edit what they disagree with, and submit. Write for someone who will act on them, not as the last word.

## 1. Establish where you are — and which path is which

**Two different paths are in play, and conflating them is the most expensive mistake available here.**

- **The pull request's repository path** — the one in the scope above. It exists to _identify_ the pull request to `get_pull_request_review_threads` and `stage_pull_request_review`. That is all it is for. It is **not** the folder you work in, and you do not `cd` into it or edit anything under it. Pass it verbatim (§2, §6) — both paths resolve to the same pending review, but substituting your own working root stops the user's open Conversation tab refreshing as your replies land.
- **Your working root** — where every file edit, git command, build and test happens. It is your current directory, which is usually **not** the path above. Establish it rather than assuming it:

```bash
git rev-parse --show-toplevel   # your working root — ALL editing and committing happens here
git rev-parse HEAD
git rev-parse --abbrev-ref HEAD
git status --short
```

Now read what you are in, because it decides how freely you may commit:

- `HEAD` must equal the head SHA in the scope above. If it does **not**, **stop** and say the tree is not on the reviewed commit — the review's line numbers do not describe this tree, and fixing the wrong lines is worse than fixing none.
- **Dispatched from the pull request's "Address review with AI" button** — you are in an isolated worktree on a session branch (`agent/<id>`), cut at that head, with its own copy of the solution. This is the normal case: commit as §5 describes. A dirty tree here is a leftover from a previous run — say what is in it and ask before building on top of it.
- **Invoked by hand** (`/address-pr-review …` typed in a chat) — there is no worktree and no session branch. You are in the user's own checkout, on their branch, quite possibly with their uncommitted work around you, and §5's commit would land there. Say that plainly and get confirmation before you commit anything, and stage only the files you changed.
- Either way, if you are on the pull request's own branch, or on `master`/`main`, **stop and say so** rather than committing there.

## 2. Read the review

`get_pull_request_review_threads` with the pull request's **repository path from the scope** — not your working root (§1) — and the pull request number. It returns the submitted reviews (whose bodies are the reviewer's framing, and often say what the inline comments are getting at) and every unresolved, non-outdated thread, each with:

- `threadId` / `replyToId` — the handles §6 needs. Carry them verbatim; never re-type or invent one.
- the file, line, side and diff hunk it is anchored to,
- whether an Intent AI review raised it, and whether an Intent AI run has **already answered it** — skip a thread already answered at this same commit rather than saying it twice.

Resolved and outdated threads are excluded on purpose. Do not go looking for them.

If there are no threads at all, say so, stage nothing, and stop.

## 3. Classify every file a comment touches — this is the step that makes it Intent's

Call `get_change_review` over the pull request's range. It tells you what each changed file **is**, and that decides how you are allowed to fix it:

| What the file is                | How to fix a comment on it                                                                                                                                                                                  |
| ------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **`FullyManaged`** (generated)  | **Fix the MODEL, then regenerate.** Never hand-edit it — the next Software Factory run reclaims your edit and the reviewer's point comes straight back. Change the designer, `run_software_factory`, apply. |
| **`ManagedWithCustomisations`** | Ask whether the model can express it. If it can, model it; only the genuinely bespoke part is hand-edited.                                                                                                  |
| **`FullyCustom`**               | Edit it directly with `patch_file`.                                                                                                                                                                         |
| **Non-code**                    | Edit it directly.                                                                                                                                                                                           |

A model change reaches the code through the Software Factory, in **your working root** (§1) — when you are in a dispatched worktree, the conversation's designers already point at that worktree's own copy of the solution, so there is nothing to redirect. Inspect the diffs before applying them, exactly as you would anywhere else.

## 4. Triage each thread, then do the work

Read the comment against the actual code — not just the diff hunk it quotes. Then it is one of three things:

- **Fix it.** The reviewer is right, or right enough that the change is an improvement either way.
- **Decline it, with a reason.** You believe the code is correct as it stands. This is a legitimate outcome and you should use it rather than making a change you think is wrong — but the reply has to be a real argument (what the reviewer may have missed, why the case they describe cannot occur), not a refusal.
- **Ask.** Only when the comment is genuinely ambiguous and the readings lead to materially different code. Use `ask_user_question` with the concrete alternatives, and carry on with the other threads while you have no answer — never stop the run on it.

Do the work thread by thread. Where several comments are really one problem, fix it once and reply to each of them saying so.

**Run the build and tests** if `run_task` offers them for this solution, and again once the last thread is done — that run is what gates §5's commit. A fix that does not compile is not a fix, and the user is about to push this.

## 5. One commit, at the end

Do **all** the editing first and commit **once**, after the build and tests pass. Never a commit per thread: the user pushes this as a single "addressed the review" change, and a branch fragmented into one commit per comment is noise in the pull request's history. Committing last also means you never commit a tree you have not built.

```bash
git add <every file you changed>
git commit -m "Address review comments on PR #<n>

- <path>:<line> — <what changed>
- <path>:<line> — <what changed>"
```

The per-thread detail goes in that message's body, so being one commit costs the history nothing.

Then `git rev-parse HEAD` **once** — every reply for a thread you changed carries that same SHA, because one commit is what happened. A declined thread changed nothing, so its reply carries none.

If you genuinely cannot finish, still commit what is done rather than leaving the worktree dirty: the push button refuses to run on uncommitted work.

## 6. Stage the replies — exactly one call, at the end

Finish with **one** `stage_pull_request_review` call:

- `repositoryRootPath` / `pullRequestNumber` — **exactly as the scope gave them.** `repositoryRootPath` is the pull request's repository path, never your working root — see §1 for why swapping it in breaks the user's open Conversation tab.
- `headSha` — your working root's HEAD **after the commit** (`git rev-parse HEAD`), not the SHA you started from.
- `replies` — one entry per thread you looked at: its `threadId` and `replyToId` from §2, a `body` saying what you did or why you did not, and `addressedInCommit` — §5's single SHA — on every thread you changed. Omit `addressedInCommit` on a declined thread.
- `headline` — one sentence saying where the review now stands ("Four threads fixed, two declined with reasons"). It renders as a single line at the top of the summary.
- `summary` — the detail below it: what was addressed, what was not and why, and anything you could not check. Facts, not narrative — no reasoning, no process, no confidence talk. It lands in the pending review's body, beneath anything the user has already typed.
- `verdict` / `findings` — leave both out. You are answering a review, not raising one.

The tool renders the footer, the "Addressed in" line and the hidden marker itself, so do not write them into a body. It also de-duplicates: a thread the pull request or the draft already carries an AI reply for is skipped. Never work around that by re-wording a reply.

Read the per-thread outcome list it returns and repeat it in chat — in particular, say which replies were skipped as duplicates and which could not be staged.

## 7. Hand back

Tell the user, in this order:

1. What you changed, thread by thread — and which threads you declined, with the reason in one line each.
2. Whether the build and tests passed.
3. Where the commit is and that it is **not yet pushed** — name the branch and, when it is a dispatched worktree, say the commit is in that worktree rather than their own checkout. They push from the pull request's Conversation tab, and then submit the pending review from the same tab so each reviewer gets one notification carrying all the replies.

Then stop. Do not push, and do not submit the review.
