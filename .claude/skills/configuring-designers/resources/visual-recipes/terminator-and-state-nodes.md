# Terminator and state nodes

Flow and state diagrams. The archived Step Functions module draws Start as an ellipse and each state as a card with a left accent bar, a small caption and a symbol. The archived modules live in `Modules.Archived/` of the `Intent.Modules` repo.

## Start (terminator)

`Element Visual Settings`: `Width`/`Height` `${w}`/`${h}`, `X`/`Y` `${x}`/`${y}`.

| Instruction | Property | Value |
| --- | --- | --- |
| `Path Draw Settings` | `Path` | `` `M ${x}, ${y + h/2} a ${w/2},${h/2} 0 1,0 ${w},0 a ${w/2},${h/2} 0 1,0 -${w},0` `` |
| | `Fill Color` | `return theme.isDark ? "#694e01" : "#fddf8b"` |
| `Text Draw Settings` | `Text` | `return 'Start';` |
| | `Style` | `` `font-size: 12px; font-family: Helvetica; font-weight: bold; text-align: center; white-space: normal; display: flex; justify-content: center; align-items: center;` `` |
| | `Width`, `Height` | `${w}`, `${h}` |
| | `X`, `Y` | `${x}`, `${y}` |

The path is an ellipse that fills the box.

## State with accent bar (Choice)

`Element Visual Settings`: `Width` `${w}`, `Height` `40`.

| Instruction | Property | Value |
| --- | --- | --- |
| `Path Draw Settings` (body) | `Path` | the rounded rectangle from `../diagram-visual-recipes.md` |
| | `Fill Color` | `return theme.isDark ? "#1f2126" : "#ffffff";` |
| `Path Draw Settings` (accent bar) | `Path` | `` `M${x + 3},${y + 6} v${h - 12}` `` |
| `Text Draw Settings` (caption) | `Text` | `return 'Choice state';` |
| | `Style` | 10px bold, left aligned |
| | `Width`, `Height` | `${w - 54}`, `14` |
| | `X`, `Y` | `${x + 50}`, `${y + 4}` |
| `Text Draw Settings` (name) | `Text` | `` return `${getName()} (Choice)`; `` |
| | `Style` | 12px bold, left aligned |
| | `Width`, `Height` | `${w - 54}`, `18` |
| | `X`, `Y` | `${x + 50}`, `${y + 20}` |
| `SVG Resource Draw Settings` | `Resource Path` | `./choice-state-symbol.svg` |
| | `Width`, `Height` | `38`, `38` |
| | `X`, `Y` | `${x + 7}`, `${y+1}` |

The accent bar is a stroke only (no fill) and takes the default line. Text starts at `x + 50` to leave room for the symbol.

A node that lists its parts (Parallel) is `container-with-rows.md`.
