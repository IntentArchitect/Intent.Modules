### Version 1.0.1

- New Feature: Added `module-svg-icon` skill that crafts a house-style SVG icon for a module from a supplied description and applies it via the module's `.application.config`, regenerated into `.imodspec` by the Software Factory.
- Improvement: `intent-module-orchestrator` now documents the `"model"` metadata bridge — how to read the originating designer element off a generated node instead of matching member names — along with the idempotency and typed-read rules that go with it.
- Improvement: `intent-module-orchestrator` now covers `TryGetTypeName` for optional, graceful-degradation integration with a module that may not be installed, contrasted with the non-optional `GetTypeName`.
- Improvement: `intent-module-orchestrator` now documents `ServiceConfigurationRequest`, `ApplicationBuilderRegistrationRequest` and `ConnectionStringRegistrationRequest` alongside their already-covered siblings, plus the "shape a type with FileBuilder, wire host infrastructure by publishing a request" rule and the lifetime-to-registration-method mapping the host owns.
- Improvement: `intent-module-orchestrator` now states the two-tier module dependency rule — role-string lookup needs no `.imodspec` dependency; reading another module's typed model interfaces does.
- Improvement: `file-builder-expert` now frames metadata as having two uses — your own cross-step state, and reading the designer model the host template already attached — instead of only the former.

### Version 1.0.0

- New Feature: Bundles a curated set of AI agent skills and module-building instructions into the consuming repo's `.agents/` folder, kept up to date on every install or update.
