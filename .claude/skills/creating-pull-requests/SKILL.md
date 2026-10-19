---
description: Open a pull request whose description proves the change works — verify it in the running app, capture screenshots, and hand them to create_pull_request, which prefills the New pull request tab or creates it directly. Use when the user asks to open, raise, create or prepare a pull request (PR).
short-description: Open a pull request with screenshots proving the change works.
argument-hint: Optionally, the base branch and whether to open it as a draft
intent-updates: automatic
requiredTools:
  - create_pull_request
  - preview_screenshot
  - bash
---

# Creating pull requests

Use this only when the user asked for a pull request. The description has to **show** the change working, not just claim it.

## 1. Verify and capture

- If the change has anything visible, load **previewing-and-testing-the-app**. Drive the app to the state that shows the change, then take a `preview_screenshot` of each state a reviewer needs to see: the new screen, the fixed behaviour, the error message. Keep each path it returns. Two or three shots is usually right. One is fine for a small change.
- Only screenshot what you checked yourself. A screenshot of the wrong state, or one that shows the bug, is worse than none.
- If nothing is visible (a backend-only change, a refactor), or the app won't run, take no screenshots. Run the tests that cover the change instead.

## 2. Commit and push

- Everything the pull request should contain must be committed. Check with `git status --short`.
- Only if you are going to call with `submit: true`: push the branch first (`git push -u origin <branch>`). The prefilled tab pushes for the user when they click Create.

## 3. Write the description

- Title: one line saying what the change does.
- Body, in markdown: what changed and why, kept short. **End it with a `## Verification` section** saying how you checked the change: what you ran, what you clicked, and which tests pass. Screenshots are added after it automatically, each under its caption.
- With no screenshots, the Verification section must say **why** there are none (e.g. "Backend-only; no UI change") and what was checked instead.

## 4. Call `create_pull_request`

- Pass `repositoryRootPath` (your working directory), `title`, `body`, and `screenshots` as `{ path, caption }`. Each caption is one line saying what the shot shows.
- Use `submit: true` **only** if the user explicitly asked you to create or open the pull request yourself. Otherwise leave it false: the tool prefills Intent's New pull request tab for the user to review and submit.
- Even with `submit: true`, the tool falls back to the prefill unless the user has turned on *Let agents create pull requests directly* in AI Configuration. If that happens, say so.
- Don't use a `gh`/`az`/`glab` CLI instead. The tool uses the user's own sign-in and uploads the screenshots to the host.

## 5. Report

- Give the pull request link if one was created. Otherwise tell the user the tab is ready to review.
- Pass on any screenshot the tool reports it could not embed, with its reason. For example, Bitbucket and GitHub Enterprise Server don't accept image uploads.
