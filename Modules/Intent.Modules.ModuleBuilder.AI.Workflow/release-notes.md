### Version 1.0.0

- Fixed: `module-version-increment` and `module-docs-chore` now document two gotchas from a real session — the Software Factory's silent no-op when a version regresses below what is on disk, and that a release-notes heading drops the `-pre.#` suffix.
- New Feature: Bundles workflow skills and instructions for the module-building lifecycle — context capture, version increments and documentation upkeep — into the consuming repo's agent skills folder.
- New Feature: Adds settings to control prerelease versioning and README maintenance in the generated skills.
- New Feature: Added a `MaintainModuleIcon` setting and icon-creation guidance to `module-docs-chore` — creates a module's SVG icon when missing (via the `module-svg-icon` skill, if available in the environment) and never overwrites an existing one.
- Improvement: Refined `module-version-increment`'s major/minor/patch rubric to judge impact on the user's experience — patch for narrow additions, minor for a meaningfully new capability dimension, major for anything that changes how users already interact with the module, even without a hard break.
- Improvement: `module-version-increment`, `module-docs-chore`, and `module-context-capture` descriptions rewritten to the Capability / `USE ONLY WHEN` / `DO NOT USE FOR` / `REQUIRES` contract for more reliable AI-harness routing.
- New Feature: Added `module-dependency-audit` as a fourth workflow skill, run at Phase 4 step 2, because a missing `.imodspec` dependency compiles cleanly and only fails once a consumer installs the module.
- New Feature: Added a `Maintain Module Context` setting — off by default, keeping the existing read-and-maintain behaviour; on, `module-context-capture` also creates a `CONTEXT.md` for a module that has none once its first durable decision lands.
- Improvement: `module-docs-chore` now checks `Module Settings → Include Release Notes` before touching release notes, and reports a ticked-but-missing file rather than silently creating one.
- Fixed: the `Intent.Common` dependency floor was 3.7.2 while the module compiles against 3.11.4 — a gap that would only surface when a consumer's install resolved the lower version.
- Improvement: Added `docs/README.md`, covering the four-phase workflow, the four module settings and how they widen the generated guidance.
- Fixed: `module-docs-chore` did not say where a module's summary and description are edited, so edits went to `.imodspec` and were silently reverted — it now names the Application Settings page and that one string fills both fields.
- Fixed: `module-version-increment`'s already-moved check applied only to ad-hoc changes, so the up-front path never consulted it and agents phantom-bumped the version on every follow-up instruction — it's now a universal gate (published-vs-local-only classification, feed-search query mechanism) that `module-building-workflow`'s Phase 2 and Phase 4 both route through.
