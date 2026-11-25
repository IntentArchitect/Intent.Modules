---
description: How to link to another module's docs from a module's own README/CONTEXT/.imodspec.
appliesTo:
  - "**/docs/README.md"
  - "**/CONTEXT.md"
  - "**/*.imodspec"
---

# Module Cross-Reference Links

When a module's `docs/README.md`, `CONTEXT.md`, or its `.imodspec` `<projectUrl>` needs to
reference another module, follow the cross-module link rule in the built-in `module-docs` skill:
link the module's published page on `docs.intentarchitect.com`, never GitHub, and confirm a derived
link resolves before using it. That rule applies to `CONTEXT.md` too. Where this rule and
`module-docs` overlap, `module-docs` wins.

This repository adds two things:

- **Why not GitHub.** A module's README lives at `Modules/{ModuleFolder}/docs/README.md`, not at the
  module folder's root. Several existing `<projectUrl>` values and "Related Modules" links omit the
  `docs/` segment, and every one of them 404s. Don't add another.
- **Fix the target's own `<projectUrl>` when it is wrong.** If it points at GitHub or 404s, replace
  it with the page you confirmed resolves. The Software Factory never overwrites that field, so a bad
  value stays until someone corrects it, and every module that copies it inherits the mistake.
