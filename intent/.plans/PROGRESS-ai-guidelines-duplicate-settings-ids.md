# PROGRESS: AI.Guidelines duplicate settings ids (branch agent/ai-skills-builtin-5-4)

## Done (uncommitted)
- Bug: AI.Workflow and AI.Guidelines 1.0.0-pre.0 declare the same settings group and setting ids. With both
  installed, every module operation on the app throws "An item with the same key has already been added.
  Key: 2ac65dde-..." (Install Agent Gate Hooks). Reproduced in Tests/ModuleBuilderSkills.
  Evidence: C:\Dev\.evidence\Intent.Modules\agent\ai-skills-builtin-5-4\2026-10-29-duplicate-settings-id\
- Fix: settings group recreated in AI.Guidelines' Module Builder designer (new ids, identical fields, values and
  comments). AI.Guidelines 1.0.0-pre.1; AI.Skills 1.2.0-pre.1 depends on AI.Guidelines 1.0.0-pre.1 (both pre.0
  are published on intentarchitect.com). Both build and package.
- A stuck app cannot escape via update, uninstall or batch uninstall, nor by removing the AI.Workflow line from
  modules.config: the duplicate lives in the app's .application.config settings. Needs an Intent fix or an
  .application.config hand edit (not authorised). Logged in INTENT-FRICTION.md.
- Tests/ModuleBuilderSkills restored to HEAD with git restore (user-authorised).

## Done 2026-10-29 (later)
- 8a851b6af (fix) and 6a2fced8b (both apps on pre.1, settings re-applied, SF 0 changes) fast-forwarded into
  development. Verified: AI.Workflow pre.12 + AI.Guidelines pre.1 co-installed, uninstall AI.Workflow succeeds.
  pre-build-validations.ps1 passes locally. Remote agent/ai-skills-builtin-5-4 still at pre-rebase 9331eeae8.

## Old next (done)
1. User restarts Intent, says "continue".
2. Test app: update AI.Guidelines pre.0 -> pre.1, install AI.Workflow 1.0.0-pre.12, uninstall AI.Workflow -> expect
   success. Restore test-app settings (all on) if reset; SF; confirm only expected changes.
3. Dogfood app: update to AI.Guidelines pre.1, restore settings (gate off, rest on), SF.
4. Docs: AI.Guidelines release notes stay one 1.0.0 entry; AI.Skills release notes mention pre.1 dependency if
   needed; AI.Guidelines CONTEXT reverses the "same settings-group id" decision; client draft wording for stuck apps.
5. Commit; ask before fast-forwarding development.
