# Context: Intent.Modelers.UI

## Purpose
A designer for describing user interfaces.

## Invariants & Constraints
- The Component Operation duplicate-signature logic is a hand-pasted copy of the compiled block from
  `DesignerMacros/src/services-operation-validation/common`. Keep it in step with that source. Only
  `findPeerOperations` is local, because it looks up `Component Operation`. See Intent.Modelers.Services'
  `CONTEXT.md` for the signature rules and the other copies.
