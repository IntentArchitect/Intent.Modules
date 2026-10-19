---
description: Keep SDD specs true when you change code they trace — check which requirements a file realises before editing it, update the spec first when the user changes traced behaviour, and run sdd-sync before finishing.
alwaysApply: true
intent-updates: automatic
---

## Keeping specs true

This applies only when the solution has specs, meaning `read_spec()` lists at least one. Each spec links its requirements to the files and model elements that realise them.

- **Before editing code with your own editor** rather than Intent's file tools, check whether the files are traced. Call `read_spec(slug, artifact: "traceability")` for each listed spec and look for the paths. For each hit, read the requirement with `read_spec(slug, artifact: "catalog", requirementId: …)`. Keep those criteria true unless the user asked you to change that behaviour.
- **A `Spec:` line in an Intent file tool's result** already names the requirements. Act on it the same way.
- **When the user asks for a change to traced behaviour, update the spec first.** Say in one line which criteria will change, e.g. "This changes ai-chat-surface R7.3." A new criterion goes in the spec whose description in `read_spec()` covers the behaviour, not just a spec that traces the file; ask when more than one fits. Patch them with `write_spec`, then call `request_spec_review(slug)`. Write the code only to the wording the user approved. If the review comes back with feedback, revise the wording and request the review again.
- **After changing traced code**, load the `sdd-sync` skill before you end the turn. It writes the spec updates, has the user review them with `request_spec_review`, and says when to skip it.
