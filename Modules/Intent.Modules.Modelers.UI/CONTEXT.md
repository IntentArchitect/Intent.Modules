# Context: Intent.Modelers.UI

## Purpose
A designer for describing user interfaces.

## Architectural Decisions
- **Diagrams keep the name they are created with, like every other element.** `New Diagram` is a Run
  Script Option that names the Diagram after its parent and opens it for renaming. The original Element
  Creation Option is kept but hidden, so Diagram stays a creatable child type for scripts. This matches
  Intent.Modelers.Domain: see its `CONTEXT.md` for the reasoning and invariants, and change all three
  modules together.

## Invariants & Constraints
- The Component Operation duplicate-signature logic is a hand-pasted copy of the compiled block from
  `DesignerMacros/src/services-operation-validation/common`. Keep it in step with that source. Only
  `findPeerOperations` is local, because it looks up `Component Operation`. See Intent.Modelers.Services'
  `CONTEXT.md` for the signature rules and the other copies.
