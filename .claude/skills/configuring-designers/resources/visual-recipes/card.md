# Card

A rounded box with a centred, bold label. The Front End model draws View, IPC Event, Process and Service this way, differing only in fill. Use it for any node whose only content is its name.

## Element Visual Settings

| Property | Value |
| --- | --- |
| `Position Settings`: `Width`, `Height` | `${w}`, `${h}` |
| `Position Settings`: `X`, `Y` | `${x}`, `${y}` |

## Draw instructions, in order

`Path Draw Settings`, stereotype `Path Settings`:

| Property | Value |
| --- | --- |
| `Path` | the rounded rectangle from `../diagram-visual-recipes.md` |
| `Line Color` | `return theme.isDark ? '#9aa4b2' : '#4a5568';` |
| `Fill Color` | a palette role, e.g. primary: `return theme.isDark ? '#1e3a5f' : '#dbeafe';` |

`Text Draw Settings`, stereotypes `Text Settings` and `Position Settings`:

| Property | Value |
| --- | --- |
| `Text` | `return getName();` |
| `Style` | `` `font-size: 12px; font-family: Helvetica; font-weight: bold; text-align: center; white-space: inherit;` `` |
| `Width`, `Height` | `${w}`, `30` |
| `X`, `Y` | `${x}`, `${y + 4}` |

## Variants

- **Group boundary:** add `Line Dash Array` = `4 2` and use the group fill (`#1f2937` / `#f0fdfa`). Place child elements inside it; see `growing-container.md`.
- **Kind by colour:** copy the same card for each kind and change only `Fill Color`.
