# Note

A sticky note with a folded top-left corner, from the Domain `Comment`. Use it for annotations that attach to other elements.

`Element Visual Settings`: `Width`/`Height` `${w}`/`${h}`, `X`/`Y` `${x}`/`${y}`. Add a `Stereotypes Visual Settings` child (`${w}`/`18` at `${x}`/`${y}`).

| Instruction | Property | Value |
| --- | --- | --- |
| `Path Draw Settings` | `Path` | `` `M${x + 10},${y} h${w - 10} v${h} h-${w} v-${h - 10} l10,-10 v10 h-10 z` `` |
| | `Fill Color` | `return theme.isDark ? '#30475E' : '#ffffdd';` |
| `Text Draw Settings` (body) | `Text` | `return comment;` (as shipped for `Comment`) |
| | `Style` | `` `font-size: 12px; font-family: Helvetica; text-align: center; white-space: pre-line;` `` |
| | `Width`, `Height` | `${w - 20}`, `${h - 40}` |
| | `X`, `Y` | `${x + 10}`, `${y + 32}` |
| `Text Draw Settings` (title) | `Style` | 12px bold centred, `white-space: normal;` |
| | `Width`, `Height` | `${w - 20}`, `${h - 16}` |
| | `X`, `Y` | `${x + 10}`, `${y + 8}` |

The path outlines the shape and the fold in one stroke, so no separate fold instruction is needed. Connect it with the dotted line in `association-lines.md`.
