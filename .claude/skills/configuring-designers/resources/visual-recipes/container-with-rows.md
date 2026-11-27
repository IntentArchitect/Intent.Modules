# Container with rows

A header with child rows stacked beneath it: the Domain `Class` (attributes, constructors, operations), and the Step Functions Parallel node. Use it for any element that lists its children inside its own shape.

## Container: `Element Visual Settings` on the parent type

| Property | Value |
| --- | --- |
| `Position Settings`: `Width`, `Height` | `${w}`, `${h}` (the shape grows with its rows) |
| `Settings`: `Auto-Resize Default` | true |

Draw instructions:

| Instruction | Property | Value |
| --- | --- | --- |
| `Path Draw Settings` | `Path` | the rounded rectangle from `../diagram-visual-recipes.md` |
| | `Fill Color` | a palette role; see `state-driven-style.md` for a conditional one |
| `Text Draw Settings` | `Style` | bold centred 12px, `padding: 0px 8px;` |
| | `Width`, `Height` | `${w}`, `18` |
| | `X`, `Y` | `${x}`, `${y + 8}` |

Leave the header `Text` empty to show the element name. Add a `Stereotypes Visual Settings` child with `Width`/`Height` `${w}`/`18` at `${x}`/`${y}` so stereotypes show on the header.

## Row: a nested `Element Visual Settings` per child type

| Property | Value |
| --- | --- |
| `Width`, `Height` | `` `${parent.w}` ``, `18` |
| `X` | `` `${parent.x}` `` |
| `Y` | `` `${parent.y + ($index * 18) + 36}` `` |

`$index` counts siblings of the same type only. When one parent lists several child types (attributes, then constructors, then operations), add the heights of the earlier groups to `Y`:

```js
`${parent.y
  + (parent.elements['Attribute'].any() ? 12 : 0) + parent.elements['Attribute'].size.height
  + ($index * 18) + 36}`
```

Row draw instructions:

| Instruction | Property | Value |
| --- | --- | --- |
| `Path Draw Settings` (separator) | `Condition` | `return $firstOfType;` |
| | `Path` | `` `M${x},${y - 6} h${w}` `` |
| `Text Draw Settings` | `Style` | `` `font-size: 12px; font-family: Helvetica; text-align: left; white-space: inherit; padding: 2px 8px 2px 0px;` `` |
| | `Width`, `Height` | `${w - 20}`, `${h}` |
| | `X`, `Y` | `${x + 20}`, `${y}` |
| `SVG Resource Draw Settings` | `Resource Path` | an icon file, e.g. `attribute.svg` |
| | `Width`, `Height` | `14`, `14` |
| | `X`, `Y` | `${x+3}`, `${y+3}` |

The separator draws only above the first row of each type. The 20px left gap is the icon column.

## Anchors for the rows

Curved lines should leave a row from its sides. On the row's `Element Visual Settings`, `Anchor Segments`:

```js
const d = getDimensions(); return [[d.getCenterLeft(), d.getCenterLeft()], [d.getCenterRight(), d.getCenterRight()]];
```

Rows shipped before `Anchor Segments` existed use the deprecated `Anchor Points` = `return [getDimensions().getCenterLeft(), getDimensions().getCenterRight()];`. Anchors apply to curved lines only, so the elbow lines the Domain module draws ignore them; see `../diagram-anchors.md`.
