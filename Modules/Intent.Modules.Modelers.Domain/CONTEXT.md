# Context: Intent.Modelers.Domain

## Purpose
A domain designer based on UML class diagrams for expressing entity relationships.

## Invariants & Constraints
- A Validate Function must `return` its message: `return validateOperation(lookup(id));`. A bare
  `validateOperation(element);` runs, but its result is discarded, so the designer never shows the error.
  This is why the Operation duplicate check reported nothing until 3.13.3.
- The Operation duplicate-signature logic is a hand-pasted copy of the compiled block from
  `DesignerMacros/src/services-operation-validation/common`. Keep it in step with that source. See
  Intent.Modelers.Services' `CONTEXT.md` for the signature rules and the other copies.
