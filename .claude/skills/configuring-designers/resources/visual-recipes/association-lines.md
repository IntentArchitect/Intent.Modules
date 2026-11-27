# Association lines

Lines and their ends, from the Domain module's `Association`, `Generalization` and `Comment Association`. Properties are on `Association Visual Settings` (stereotype `Setting`) and on its two end elements, `Association Source End Visual Settings` and `Association Destination End Visual Settings`; see `../diagram-visuals.md` for what each stereotype holds.

## Plain line

`Setting`: `Line Type` = `Elbow Connector` (or `Curved`, which can spread; see `../diagram-anchors.md`). Leave the rest unset for a solid default line.

## UML association: multiplicity, navigability, dashes

`Setting` → `Line Dash Array`, a function: `return null;` for solid, `return "5,5";` for dashed. The Domain module returns solid when the far end is required and single, and dashed for a reference to a document store.

Each end's `Label Settings`:

| Property | Value |
| --- | --- |
| `Primary Label` | `` return `${name && typeReference.isNavigable ? name : ""}`; `` (the role name, when navigable) |
| `Secondary Label` | `` return `${typeReference.isCollection ? "*" : typeReference.isNullable ? "0..1" : "1"}`; `` (multiplicity) |

The end's context gives `name` and `typeReference` (`isCollection`, `isNullable`, `isNavigable`).

Marker paths are SVG path fragments relative to the end of the line:

| Marker | Element | Property | Value |
| --- | --- | --- | --- |
| Diamond (composition when filled, aggregation when open) | source end `Point Settings` | `Path` | `` return `l 5.0 7.5 l -5.0 7.5 l -5.0 -7.5 Z`; `` |
| | | `Fill Color` | `` return !typeReference.isNullable && !typeReference.isCollection ? '#000000' : '#ffffff'; `` |
| Arrow for a navigable end | `Navigable Indicator Settings` | `Path` | `` return `l 5 10 l -5 -2.5 l -5 2.5 Z`; `` (source end: `m 0 15 …`) |
| | | `Fill Color` | `return '#000000';` |

## Inheritance

Destination end `Point Settings`: `Path` = `` return `l 7 10 l -14 0 Z`; ``, `Fill Color` = `return '#ffffff';`. A hollow triangle at the parent.

## Dotted annotation

`Setting` → `Line Dash Array` = `3,8`, and a destination end `Point Settings` with `Path` = `` return `l 3 5 l -6 0 l 3 -5 Z`; ``. Use it from a note (`note.md`) to the thing it annotates.

## Theme

The shipped markers use fixed black and white fills, not theme-aware ones. Check them in both themes on your diagram.
