# Context: Intent.Metadata.Domain.Constraints

## Purpose
Adds stereotypes to Domain designer Attributes to describe entity attribute constraints. It also ships the
generated `AttributeModelStereotypeExtensions` accessors that other modules use to read them.

## Architectural Decisions
- **Compile against the published `Intent.Modules.Modelers.Domain` package, not the project.** The
  version is pinned to match the `Intent.Modelers.Domain` dependency in the `.imodspec` (3.12.14). The
  module originally used a ProjectReference to `Intent.Modules.Modelers.Domain`. That leaked the
  in-development Domain module's SDK and `Intent.Modules.Common` versions into this build. When Domain
  moved to SDK 3.14.0-pre.0 and Common 3.11.8-pre.0, restore failed with NU1605 downgrade errors.
  Rejected: raising this module's own SDK and Common references to match. SDK 3.14 needs Intent
  Architect 4.7+, so this module's `supportedClientVersions` floor (4.4.0) would have to rise for no
  functional gain. Every other module already consumes Modelers.Domain as a published PackageReference.

## Invariants & Constraints
- Keep the `Intent.Modules.Modelers.Domain` PackageReference version in step with the `.imodspec`
  `Intent.Modelers.Domain` dependency. Its transitive SDK and Common versions must not exceed this
  module's own references, or NU1605 returns.
