---
description: Work through a pull request's unresolved review comments — fix what should be fixed, commit each one, and stage a reply per thread into the pending review. Use when asked to address, action or respond to review feedback on a pull request.
short-description: Work through a pull request's unresolved review comments — fix, commit, and reply per thread.
argument-hint: The pull request — a number, a repository path, and the head SHA the worktree was cut at
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

- **Commit, never push.** You commit your own work; the user pushes it from the pull request's Conversation tab. Never `git push`, never rewrite history (no `commit --amend`, no `rebase`, no `reset --hard`), never touch a branch other than the one you are on.
- **Reply to every thread you looked at**, including the ones you decided not to change. A thread with no reply reads as a thread you missed.
- **Nothing reaches the git host.** `stage_pull_request_review` puts your replies in the user's own PENDING review, on their machine. They read them, edit what they disagree with, and submit. Write for someone who will act on them, not as the last word.

## 1. Confirm you are where you think you are

You are running in an isolated worktree cut at the pull request's head. Prove it before you change anything:

```bash
git rev-parse HEAD
git status --short
git rev-parse --abbrev-ref HEAD
```

- `HEAD` must equal the head SHA in the scope above. If it does **not**, **stop** and tell the user the worktree is not on the reviewed commit — the review's line numbers do not describe this tree, and fixing the wrong lines is worse than fixing none.
- The branch should be the session branch (`agent/<id>`). If you are on the pull request's own branch, or on `master`/`main`, **stop and say so** rather than committing there.
- A dirty tree is a leftover from a previous run. Say what is in it and ask before you build on top of it.

## 2. Read the review

`get_pull_request_review_threads` with the repository path and the pull request number. It returns the submitted reviews (whose bodies are the reviewer's framing, and often say what the inline comments are getting at) and every unresolved, non-outdated thread, each with:

- `threadId` / `replyToId` — the handles §6 needs. Carry them verbatim; never re-type or invent one.
- the file, line, side and diff hunk it is anchored to,
- whether an Intent AI review raised it, and whether an Intent AI run has **already answered it** — skip a thread already answered at this same commit rather than saying it twice.

Resolved and outdated threads are excluded on purpose. Do not go looking for them.

If there are no threads at all, say so, stage nothing, and stop.

## 3. Classify every file a comment touches — this is the step that makes it Intent's

Call `get_change_review` over the pull request's range. It tells you what each changed file **is**, and that decides how you are allowed to fix it:

| What the file is                | How to fix a comment on it                                                                                                                                                                              |
| ------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **`FullyManaged`** (generated)  | **Fix the MODEL, then regenerate.** Never hand-edit it — the next Software Factory run reclaims your edit and the reviewer's point comes straight back. Change the designer, `run_software_factory`, apply. |
| **`ManagedWithCustomisations`** | Ask whether the model can express it. If it can, model it; only the genuinely bespoke part is hand-edited.                                                                                              |
| **`FullyCustom`**               | Edit it directly with `patch_file`.                                                                                                                                                                     |
| **Non-code**                    | Edit it directly.                                                                                                                                                                                       |

A model change reaches the code through the Software Factory, in **this worktree** — the conversation's designers already point at the worktree's own copy of the solution. Inspect the diffs before applying them, exactly as you would anywhere else.

## 4. Triage each thread, then do the work

Read the comment against the actual code — not just the diff hunk it quotes. Then it is one of three things:

- **Fix it.** The reviewer is right, or right enough that the change is an improvement either way.
- **Decline it, with a reason.** You believe the code is correct as it stands. This is a legitimate outcome and you should use it rather than making a change you think is wrong — but the reply has to be a real argument (what the reviewer may have missed, why the case they describe cannot occur), not a refusal.
- **Ask.** Only when the comment is genuinely ambiguous and the readings lead to materially different code. Use `ask_user_question` with the concrete alternatives, and carry on with the other threads while you have no answer — never stop the run on it.

Do the work thread by thread. Where several comments are really one problem, fix it once and reply to each of them naming the same commit.

**Run the build and tests** if `run_task` offers them for this solution, and again before you finish. A fix that does not compile is not a fix, and the user is about to push this.

## 5. One commit per thread

Commit as you go, not in one lump at the end — the replies name commits, and a single commit cannot tell four threads apart.

```bash
git add <the files for THIS thread>
git commit -m "<what changed>

Addresses review comment on <path>:<line>.
Refs PR #<n>"
```

Then `git rev-parse HEAD` for the SHA the reply will carry. A declined thread gets no commit, and its reply carries no SHA.

## 6. Stage the replies — exactly one call, at the end

Finish with **one** `stage_pull_request_review` call:

- `repositoryRootPath` / `pullRequestNumber` — as the scope gave them.
- `headSha` — the worktree's HEAD **after your last commit** (`git rev-parse HEAD`), not the SHA you started from.
- `replies` — one entry per thread you looked at: its `threadId` and `replyToId` from §2, a `body` saying what you did or why you did not, and `addressedInCommit` for the ones you changed. Omit `addressedInCommit` on a declined thread.
- `summary` — what was addressed, what was not and why, and anything you could not check. It lands in the pending review's body, beneath anything the user has already typed.
- `findings` — leave it out. You are answering a review, not raising one.

The tool renders the footer, the "Addressed in" line and the hidden marker itself, so do not write them into a body. It also de-duplicates: a thread the pull request or the draft already carries an AI reply for is skipped. Never work around that by re-wording a reply.

Read the per-thread outcome list it returns and repeat it in chat — in particular, say which replies were skipped as duplicates and which could not be staged.

## 7. Hand back

Tell the user, in this order:

1. What you changed, thread by thread, with the commit for each — and which threads you declined, with the reason in one line each.
2. Whether the build and tests passed.
3. That the commits are **on the session branch and not yet pushed**: they push them from the pull request's Conversation tab, and then submit the pending review from the same tab so each reviewer gets one notification carrying all the replies.

Then stop. Do not push, and do not submit the review.
