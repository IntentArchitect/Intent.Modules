---
description: How to link to another module's docs from a module's own README/CONTEXT/.imodspec.
appliesTo:
  - "**/docs/README.md"
  - "**/CONTEXT.md"
  - "**/*.imodspec"
---

# Module Cross-Reference Links

When a module's `docs/README.md`, `CONTEXT.md`, or its `.imodspec` `<projectUrl>` needs to
reference another module — a "Related Modules" section, a "see also", an install-requirement
callout — link to its published page on `docs.intentarchitect.com`. Never link to GitHub.

## Why not GitHub

A module's README lives at `Modules/{ModuleFolder}/docs/README.md` — not at the module folder's
root. Several existing modules' `<projectUrl>` and hand-written "Related Modules" links were built
as `https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/{ModuleFolder}/README.md`,
omitting the `docs/` segment — every one of those 404s. Don't add another one; use the doc site
instead of a repo-relative guess.

## The URL format
https://docs.intentarchitect.com/articles/{section}/{slug}/{slug}.html


`{slug}` is the module's own id — the value in its `.imodspec`'s `<id>` element (e.g.
`Intent.Application.Wolverine`) — lowercased, with every `.` replaced by `-`. Nothing else changes:
no extra hyphenation is inserted inside a compound word.

| Module id | Slug |
|---|---|
| `Intent.Application.Wolverine` | `intent-application-wolverine` |
| `Intent.Eventing.NServiceBus` | `intent-eventing-nservicebus` |
| `Intent.Application.Dtos.ObjectMapping` | `intent-application-dtos-objectmapping` |

`{section}` is usually `modules-dotnet`, but not always:

- Regular runtime/code-generation modules (the vast majority) → `modules-dotnet`
- Designer-only ("Modelers") modules are inconsistent — most are `modules-dotnet` too, but some
  (e.g. `Intent.Modelers.Eventing`) are under `modules-common` instead. There is no reliable rule
  to derive this from the module id alone — it has to be checked.

## Always verify before publishing a link

1. **Check the target module's own `.imodspec` for an existing `<projectUrl>` first.** If it has
   one, copy it verbatim — that's the confirmed value, not something to re-derive.
2. **If it has none, derive the slug and section per the rules above and confirm the page actually
   resolves** (fetch it) before using it in another module's docs. Not every module in this repo is
   published yet — one still at an early `-pre.#` may have no live page. A derived link that hasn't
   been checked is worse than no link: reference the module by name in plain text instead of
   guessing, and if `modules-dotnet` 404s, retry under `modules-common` before giving up.
3. **If the target module's own `<projectUrl>` is missing or wrong** (points at GitHub, or 404s),
   fix it there too while you're at it — `<projectUrl>` is a hand-maintained field the Software
   Factory never overwrites, so a bad value sits there indefinitely until someone corrects it, and
   every other module's "Related Modules" section that links to it inherits the mistake.
