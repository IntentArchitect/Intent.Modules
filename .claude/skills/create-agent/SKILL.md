---
description: Create or edit a custom Intent Architect agent (persona) — a `.agent.md` file whose frontmatter sets its name, icon, picker visibility, sub-agent dispatch and tool list, and whose body is its system prompt. Use when the user asks to create, generate, design or change a custom agent or persona, or runs /create-agent.
short-description: Create or edit a custom agent's .agent.md file.
argument-hint: "[what the agent should do]"
intent-updates: automatic
requiredTools:
  - read_file
  - write_file
  - patch_file
  - glob
group: Documents, preview and tasks
---

# Creating an agent

A custom agent is one markdown file, `<id>.agent.md`, in the solution's `.agents/agents` folder (the folder beside the `.isln`). The file name minus `.agent.md` is the agent's id; the user switches to it in the chat's agent picker or by sending `/<id>`. Nothing is modelled or generated — this is an ordinary file edit.

## 1. Pin down what it is for

If the request doesn't say, ask (with `ask_user_question`, at most one round):

- **The job** — what the user will ask it to do, and what "done" looks like.
- **Read-only or writing** — does it only explore and answer, or change files / the model?
- **Who starts it** — the user from the picker, other agents as a sub-agent, or both.

Don't ask what you can decide: pick the id, name, icon and tools yourself.

## 2. Read what's there

`glob` `.agents/agents/*.agent.md` under the solution. Don't reuse an existing id unless the user asked to change that agent. The built-in personas (`agent`, `plan`, `ask`, `discovery`) are good references for tool lists. A custom file with a built-in's id replaces it, so never use one of those ids by accident.

## 3. Write the file

```markdown
---
name: Migration Reviewer
description: Reviews EF Core migrations for data loss and locking risks.
icon: fa-database
userInvocable: true
modelInvocable: true
tools:
  - read_file
  - glob
  - grep
  - bash
---

You review database migrations. …
```

| Key               | Meaning                                                                                                                                                                   |
| ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `name`            | Label in the agent picker. Defaults to the id.                                                                                                                            |
| `description`     | One line. Shown in the picker and in AI Configuration › Agents, and read by other agents deciding whether to dispatch it.                                                 |
| `icon`            | A Font Awesome 4 class, e.g. `fa-search`, `fa-shield`, `fa-flask`.                                                                                                        |
| `userInvocable`   | `true` (default): listed in the agent picker. `false` for a worker only other agents dispatch.                                                                            |
| `modelInvocable`  | `true` (default): other agents may dispatch it with `create_sub_agent`.                                                                                                   |
| `tools`           | The Intent tools it may call. Omit a tool to deny it.                                                                                                                     |
| `model`, `effort` | Leave out (or `inherit`). Model keys are per-user, so a shared file can't name one. `effort` is `low`/`medium`/`high` where a dispatcher should run it cheaper or harder. |

**Tools.** Grant the fewest that do the job:

- Read-only: `read_file`, `glob`, `grep`, `search_docs`, plus `get_applications`, `run_designer_query`, `run_solution_query` for the model, and `read_spec` for specs.
- Writing code: add `write_file`, `patch_file`, `delete_code_file`, `run_task`.
- Changing the model: add `get_designer_rules`, `run_designer_script`, `run_software_factory`, `apply_staged_file_changes`.
- Asking the user: `ask_user_question`. Shell and git: `bash`, `powershell`.

**The body** is the system prompt. Write it to the agent, in the second person: its job, how it works step by step, what it must not do, and what its final answer looks like. Point at skills by name (`load the review-changes skill`) rather than copying their rules in. Keep it under a page; the agent also gets Intent's own rules and skills.

The body must not be empty and the frontmatter must open on the first line, or the file is ignored.

## 4. Check it loads

`read_file` the file back and check the frontmatter parses as YAML (list items indented under `tools:`, no tabs). Tell the user its id and that it appears in the agent picker (or, with `userInvocable: false`, only as a sub-agent), and that AI Configuration › Agents lists it. The picker reloads its agents each time it is opened.
