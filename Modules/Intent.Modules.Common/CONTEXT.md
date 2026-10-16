# Context: Intent.Common

## Purpose
The base library every module builds on: template and registration base classes, the file builders
(including `MarkdownFileBuilder`), metadata helpers and the cross-module event plumbing. It is consumed as
a code library by every module, so its behaviour is a contract with modules this repository cannot see.

## Architectural Decisions
- **`MarkdownFileParser` keeps a wrapped list item together** (3.11.8). A non-blank line straight after a
  list item continues it, as in Markdown's own "lazy continuation" rule, unless it starts a block of its own
  (heading, fence, list item, `|`, `>`, a thematic break, an HTML block, or an item-like marker such as `1a.`
  or `2)` that the parser does not model). The parser stores each continuation line already indented to
  the item's content column, keeping trailing spaces (a hard break). Rejected after an adversarial review:
  re-indenting in `MarkdownFile.ToString`, which also changed the output of multi-line items built through
  the API (flattening code inside them). Rendering is unchanged; only parsing is. Before, the rest of the sentence became a text block written after a blank
  line, splitting the bullet; every skill the ModuleBuilder.AI modules ship carried such splits, and their
  templates had been told to keep each bullet on one physical line. Rejected: fixing it in the templates -
  the parser is where the meaning was lost, and every other markdown template has the same exposure.
- **List levels are measured from the first item of each run of list items.** A list indented as a whole
  (four spaces in under a paragraph) used to be written back four spaces in, where Markdown reads it as a
  code block. A run restarts at a list item that follows an UNINDENTED block (or starts a section); after
  an indented paragraph or fence - one inside the previous item - the run keeps its base, so a nested list
  there stays nested.
- **Checked against every markdown template in Intent.Modules.NET (36) and this repository's AI modules**
  by rendering each through the old and the new parser: Intent.Modules.NET's output is byte-identical; the
  AI modules' only differences are the intended ones.

## Invariants & Constraints
- A parsed list item's `Content` may hold several lines separated by `\n`, continuation lines already indented.
  `MarkdownFile.ToString` writes `Content` verbatim, as before, so items built through the API render as
  they always did.
- Two adjacent lists separated only by a blank line are still written without it (unchanged behaviour).
