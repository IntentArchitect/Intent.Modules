# Context: Intent.Modelers.Domain

## Purpose
A domain designer based on UML class diagrams for expressing entity relationships.

## Architectural Decisions
- **Diagrams keep the name they are created with, like every other element.** The user-facing `New
  Diagram` options are Run Script Options. They create the Diagram named after its parent, load it and call
  `enableEditing()`, the same pattern as the UI designer's `New Page`. The original Element Creation
  Options are kept but hidden (`Is Option Visible Function` returns false), so Diagram stays a creatable
  child type for scripts. The Diagram element has no On Created handler, and the package's On Created
  handler names its first Diagram after the package directly.
  Rejected: an On Created handler that set the parent's name and called `enableEditing()`. It ran on every
  creation, after `loadDiagram()` resolved, so it replaced the name a script (e.g. an AI agent) had already
  set. Also rejected: keeping that handler but renaming only default names (`NewDiagram`, `Default Diagram`).
  That fixed scripts, but guessed intent from the name, and no other element works that way. Trade-off:
  Run Script Options render after Element Creation Options, so `New Diagram` moved to the end of the
  context menu.

## Invariants & Constraints
- A Validate Function must `return` its message: `return validateOperation(lookup(id));`. A bare
  `validateOperation(element);` runs, but its result is discarded, so the designer never shows the error.
  This is why the Operation duplicate check reported nothing until 3.13.3.
- The Operation duplicate-signature logic is a hand-pasted copy of the compiled block from
  `DesignerMacros/src/services-operation-validation/common`. Keep it in step with that source. See
  Intent.Modelers.Services' `CONTEXT.md` for the signature rules and the other copies.
- The `New Diagram` Run Script Option script is copied, identically, into Intent.Modelers.Services and
  Intent.Modelers.UI, once per Package and Folder context menu. Change every copy together.
- Never delete a hidden `New Diagram` Element Creation Option. It is what makes Diagram an accepted child
  of the Package and Folder, so removing it stops scripts (and AI agents) from creating Diagrams there.
- Code that creates a Diagram must name it and, if needed, call `loadDiagram()` itself. Nothing does
  either on creation any more.
