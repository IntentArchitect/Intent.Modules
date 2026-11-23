### Version 1.0.1

- Improvement: A finding about a member names it with what owns it, `Order.Total`, each part a link to its element, in the report and in the log.
- Fixed: The drift report AI instruction, which every application in a solution writes to the same file, named the report path of whichever application ran last, so each application's Software Factory run changed it. It now names `.cache/drift/<Application Name>/DriftReport.mdx`.

### Version 1.0.0

- New Feature: One drift report per application. Modules that check the model and the code for drift subclass `DriftReporterBase` and add their findings to named Areas; this module runs them after template execution, logs one error when there are drift errors, or one warning when there are only drift warnings, with each Area's counts and the report's path, and stages the report at `.cache/drift/<application>/DriftReport.mdx` when there are findings. In a CI/CD run (`--param ensure-no-outstanding-changes=true`) the report isn't written, so the error lists every finding, and a drift error ends the run with an exception once every finding is logged, so the pipeline fails.
- New Feature: An always-on AI instruction that tells agents where drift findings appear after a run and how to read the report.
