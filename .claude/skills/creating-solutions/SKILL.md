---
description: How to create a new, empty Intent Architect solution with create_solution — in an agent conversation, for a folder (e.g. a brownfield repository) that has no solution yet; from an external MCP client, from the home screen. Load before creating a solution. Applications are added separately with create_application.
short-description: Create a solution for a folder that has none (then add applications).
intent-updates: automatic
---

# Creating Solutions

`create_solution` creates a **new, empty solution** (no application). Adding applications is a **separate** step: follow the **creating-applications** skill (`create_application`) afterwards.

Never invent the solution **name** — confirm it with the user, suggesting the repository folder's name. When an application is being added straight after, ask for the solution name in the same `ask_user_question` call as the application's architecture and name (creating-applications' Call 1) rather than in a call of its own.

## In an agent conversation

Use it when the working folder has **no** Intent solution — the model tools refuse with "no Intent model to read or change here". Pass only `solutionName` (and optionally `description`); the location options are ignored.

- It writes `<repository root>/intent/<solutionName>.isln` (the conversation's folder when it is not in a git repository), adds the `.gitignore` entries, and does not run `git init`.
- It refuses when the folder already has a solution, when `<repository root>/intent` already holds one, when the name is not a plain file name, or when a folder window is open on a subfolder of the repository.
- Nothing is opened: the next model tool call already resolves the new solution. A window open on that folder adopts it in place.

## From an external MCP client (no conversation)

Use it only from the home screen, before any solution is open. Options, keeping the defaults unless the user asks otherwise:

- `solutionName` (required), `description` (optional).
- `location` — base directory; omit for the user's configured Application Location.
- `createSubFolderForSolution` (default true), `storeMetadataInIntentFolder` (default true), `setGitIgnoreEntries` (default true), `initializeGitRepository` (default true).

The solution is created and opened.

In both modes it does **not** create an application, install modules, or run the Software Factory — so there is no `run_software_factory` step here.
