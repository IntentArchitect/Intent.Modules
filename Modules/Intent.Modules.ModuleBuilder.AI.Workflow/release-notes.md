### Version 1.0.0

- New Feature: Bundles workflow skills and instructions for the module-building lifecycle — context capture, version increments and documentation upkeep — into the consuming repo's agent skills folder.
- New Feature: Adds settings to control prerelease versioning and README maintenance in the generated skills.
- New Feature: Added a `MaintainModuleIcon` setting and icon-creation guidance to `module-docs-chore` — creates a module's SVG icon when missing (via the `module-svg-icon` skill, if available in the environment) and never overwrites an existing one.
- Improvement: Refined `module-version-increment`'s major/minor/patch rubric to judge impact on the user's experience — patch for narrow additions, minor for a meaningfully new capability dimension, major for anything that changes how users already interact with the module, even without a hard break.
- Improvement: `module-version-increment`, `module-docs-chore`, and `module-context-capture` descriptions rewritten to the Capability / `USE ONLY WHEN` / `DO NOT USE FOR` / `REQUIRES` contract for more reliable AI-harness routing.
