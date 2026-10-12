# Context: Intent.Modelers.Services.CQRS

## Purpose
Adds Commands and Queries to the Services designer, with their element settings, interactions and the Services
package and folder extensions that create them. It is designer metadata only and generates no code. The modules
that implement Commands and Queries (`Intent.Application.MediatR`, `Intent.Application.Wolverine`) generate the
code.

## Architectural Decisions
- **No implementation-specific validation on Command or Query.** The "default value order" warning (a property
  without a default value after one with a default value) was the whole Validate Function of both element settings
  until 6.0.8. It moved to `Intent.Application.MediatR` and `Intent.Application.Wolverine` as Element Extensions,
  because their constructor templates drop out-of-order defaults. In a Services-only application, or one whose
  dispatch generates no constructor, the warning was noise. Rejected: keeping it here behind a check for those
  modules. That would put knowledge of the implementing modules into the designer module, and a new implementing
  module would need a change here. Don't add a Validate Function back to Command or Query for a rule that only one
  implementation's output needs.

## Module Interactions
- **Intent.Application.MediatR** and **Intent.Application.Wolverine**: this module's `<interoperability>` updates them
  to 4.7.6 and 1.1.2 (written against `4.7.6-pre.0` and `1.1.2-pre.0`), the first versions that carry the warning.
  Each raises its dependency floor on this module to 6.0.9 in return. With both directions pinned, an application
  never runs both copies of the warning (a duplicate) or neither. A new module that implements Commands and Queries
  with a constructor should add the same Element Extension and an entry in this `<interoperability>` block.
