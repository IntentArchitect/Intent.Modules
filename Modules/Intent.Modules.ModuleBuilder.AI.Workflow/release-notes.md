### Version 1.1.0

- New Feature: Added `module-dependency-audit` and wired it into `module-building-workflow` as the fourth workflow skill, run at Phase 4 step 2. A missing `.imodspec` dependency compiles cleanly and only fails when a consumer installs the module, so a green build never catches it.
- New Feature: Added a `Maintain Module Context` setting. Off (the default) keeps today's behaviour of only maintaining an existing `CONTEXT.md`; on, `module-context-capture` also creates one for a module that has none, once its first durable decision lands.
- Improvement: `module-docs-chore` now checks `Module Settings → Include Release Notes` before touching release notes, and reports a ticked-but-missing file rather than silently creating one.
- Fixed: `module-docs-chore` did not say where a module's summary and description are edited, so the change was made in `.imodspec` and silently reverted. It now names the Application Settings page, and explains that one string fills both fields.

### Version 1.0.0

- Fixed: `module-version-increment` and `module-docs-chore` now document two gotchas found from a real session going wrong — the Software Factory's silent no-op when a version change regresses below what's on disk (the "downgrade guard"), and the rule that a release-notes heading drops the `-pre.#` suffix even when the module's own version keeps it.
- New Feature: Bundles workflow skills and instructions for the module-building lifecycle — context capture, version increments and documentation upkeep — into the consuming repo's agent skills folder.
- New Feature: Adds settings to control prerelease versioning and README maintenance in the generated skills.
- New Feature: Added a `MaintainModuleIcon` setting and icon-creation guidance to `module-docs-chore` — creates a module's SVG icon when missing (via the `module-svg-icon` skill, if available in the environment) and never overwrites an existing one.
- Improvement: Refined `module-version-increment`'s major/minor/patch rubric to judge impact on the user's experience — patch for narrow additions, minor for a meaningfully new capability dimension, major for anything that changes how users already interact with the module, even without a hard break.
- Improvement: `module-version-increment`, `module-docs-chore`, and `module-context-capture` descriptions rewritten to the Capability / `USE ONLY WHEN` / `DO NOT USE FOR` / `REQUIRES` contract for more reliable AI-harness routing.
