# Icon card

A card with an icon above the label, as the Services Domain Interactions `Repository` draws it.

## SVG resource

The module keeps the file in its `resources/` folder (see `diagram-visuals.md`).

| Instruction | Property | Value |
| --- | --- | --- |
| `SVG Resource Draw Settings` (`SVG Resource Settings`) | `Resource Path` | the file name, e.g. `respository.svg` |
| | `Width`, `Height` | `${w}`, `36` |
| | `X`, `Y` | `${x}`, `${y + 2}` |
| `Text Draw Settings` (`Text Settings`) | `Style` | `` `font-size: 12px; font-family: Helvetica; font-weight: bold; text-align: center; white-space: inherit; vertical-align: center` `` |
| | `Width`, `Height` | `${w}`, `16` |
| | `X`, `Y` | `${x}`, `${y + 40}` |

The element is 60 high (`Position Settings` `Height` = `60`) with no outline, so the icon is the shape. Leave `Text` empty to show the element name.

The Front End model's Service card draws its icon differently: a base64 SVG as the background of an empty `Text Draw Settings`, so it needs no resource file. Prefer the resource file; it is easier to edit.
