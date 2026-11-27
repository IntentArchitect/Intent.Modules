# Diagram visual recipes

Proven starting points for how an element or association looks on a diagram. Each recipe below is lifted from a shipped module and names it, so you can open that module's Module Builder model for the full version. None was re-rendered while writing this skill: confirm the result on a diagram, in light and dark, before calling it done.

Property labels are the Module Builder's (see `scripting-the-module-builder.md`); what each draws is in `diagram-visuals.md`. Start from the use case, take the recipes it lists, then apply the shared foundations below.

## Use cases

| Diagram | Recipes |
| --- | --- |
| System, component or screen map | `card.md`, `icon-card.md`, `growing-container.md` for a boundary or group, `note.md`, plain lines in `association-lines.md` |
| Entity or class model | `container-with-rows.md`, `state-driven-style.md`, UML ends and multiplicity in `association-lines.md`, `note.md` |
| Process, flow or state diagram | `terminator-and-state-nodes.md`, `container-with-rows.md` for a node that lists parts, `note.md`, plain lines |
| Services and their operations | `icon-card.md` (the shipped `Repository` and Front End `Service` list their operations as rows under the icon; copy those rows from the module) |

## Recipes

Files are in `visual-recipes/`.

| File | Draws | Shipped in |
| --- | --- | --- |
| `card.md` | Rounded box with a centred label, one fill per kind; dashed variant for a group | Front End model |
| `icon-card.md` | Card with an icon above the label, two ways to draw the icon | Services Domain Interactions, Front End model |
| `container-with-rows.md` | Header plus stacked child rows with separator and per-row icon | Domain (`Class`) |
| `growing-container.md` | Container whose height follows its child count | Front End model (`Preference Section`) |
| `terminator-and-state-nodes.md` | Ellipse start, state node with accent bar and symbol | Step Functions (archived) |
| `note.md` | Sticky note with a folded corner | Domain (`Comment`) |
| `state-driven-style.md` | Colour or icon chosen from the element's data | Domain (`Class`, `Operation`) |
| `association-lines.md` | Multiplicity labels, navigability, inheritance triangle, dashed and dotted lines | Domain |

## Shared foundations

**One shape.** A rounded rectangle with a 5px corner. As a `Path` value:

```js
return `M${x + 5},${y} h${w - 10} a5,5 0 0 1 5,5 v${h - 10} a5,5 0 0 1 -5,5 h-${w - 10} a5,5 0 0 1 -5,-5 v-${h - 10} a5,5 0 0 1 5,-5 z`;
```

**Palette.** Every colour is a function that picks per theme, so one value works in both: `return theme.isDark ? '<dark>' : '<light>';`. These pairs come from the Front End model and the Domain module:

| Role | Dark | Light |
| --- | --- | --- |
| Line (all boxes) | `#9aa4b2` | `#4a5568` |
| Neutral fill | `#1f2937` | `#e5e7eb` |
| Primary (screen, view) | `#1e3a5f` | `#dbeafe` |
| Warning (service) | `#3f3a1e` | `#fef3c7` |
| Event | `#2e1f4f` | `#ede9fe` |
| Group (container) | `#1f2937` | `#f0fdfa` |
| Group child | `#134e4a` | `#ccfbf1` |

Pick a role by meaning, and keep one colour per kind across the diagram. Leave `Line Color` unset to get the default outline.

**Text.** `Style` is a backtick CSS string. Use 12px Helvetica throughout: bold and centred for a title, left aligned with `padding: 2px 8px` for a row, 10px bold left for a small caption. Include `white-space: inherit;`.

**Anchors.** Rows that lines attach to need left/right anchors; see `diagram-anchors.md`.

## Check

Add three or four of the elements to a diagram with several lines between them. Screenshot it in light and in dark. Check the text fits, the fills are readable, and lines meet the shape where you expect.
