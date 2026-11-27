# Growing container

A boundary or group that holds child cards and gets taller as children are added. The Front End model's `Preference Section` is a dashed box with a title and one teal card per `Preference`.

## Container: `Element Visual Settings`

| Property | Value |
| --- | --- |
| `Width`, `Height` | `${w}`, `` return `${45 + element.getChildren('Preference').length * 40}`; `` |
| `X`, `Y` | `${x}`, `${y}` |

Set `Settings` → `Auto-Resize Default` to true: that is what makes a top-level container follow its children as they are added. The `Height` expression only sets the starting size (use `45 + count * rowHeight`, as the shipped one does: a title band plus 40 per 30px row and its gap); a top-level shape does not re-evaluate it afterwards.

Draw instructions:

| Instruction | Property | Value |
| --- | --- | --- |
| `Path Draw Settings` | `Path` | the rounded rectangle from `../diagram-visual-recipes.md` |
| | `Line Color` | `return theme.isDark ? '#9aa4b2' : '#4a5568';` |
| | `Line Dash Array` | `4 2` |
| | `Fill Color` | `return theme.isDark ? '#1f2937' : '#f0fdfa';` |
| `Text Draw Settings` | `Text` | `return getName();` |
| | `Style` | bold centred 12px |
| | `Width`, `Height` | `${w}`, `18` |
| | `X`, `Y` | `${x}`, `${y + 4}` |

## Child: nested `Element Visual Settings`

| Property | Value |
| --- | --- |
| `Width`, `Height` | `${parent.w - 20}`, `30` |
| `X` | `${parent.x + 10}` |
| `Y` | `` return `${parent.y + 30 + $index * 40}`; `` |

The child is a card (`card.md`) in the group-child fill (`#134e4a` / `#ccfbf1`), with a 12px centred, non-bold label at `y + 7`.

## Notes

- `$index` is the child's position among siblings of its own type.
- A `Height` or `Y` value that is code must start with `return`, as above. A plain `${...}` template is also accepted for position and size.
