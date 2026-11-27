# Diagram visuals

How elements and associations are drawn on a diagram. Anchors (where lines attach) are in `diagram-anchors.md`.

## Where it is configured

| Model element | Persisted | Applies to |
| --- | --- | --- |
| `Element Visual Settings` under `Element Settings` | `<visualSettings type typeId>` | The element type itself. |
| `Diagram Settings` under `Element Settings` / `Element Extension` | `<diagramSettings>` | Diagram menu and visuals for types the diagram offers; holds `Context Menu`, `Element Visual Settings`, `Association Visual Settings`. |
| Nested `Element Visual Settings` | `<childElementVisualSettings>/<childElementVisualSetting>` | A child row inside its parent's shape (class attributes, operations). |
| `Association Visual Settings` under `Association Settings` | `<visualSettings>` | The line for that association type. |

Not modelled: `<addNewElementsTo>` (`Package` or `Self`), which sets where a diagram adds new elements. It is persisted and honoured, but has no Module Builder field.

## Element Visual Settings

Stereotypes:

| Stereotype | Properties | Persisted |
| --- | --- | --- |
| `Settings` | `Auto-Resize Default`, `Anchor Segments`, `Anchor Spread`; `Anchor Points` is deprecated, and older Module Builders have only it (see `diagram-anchors.md`) | `<anchorSegmentsFunction>`, `<anchorSpread>`, `<anchorPointsFunction>`, `<autoResizeEnabledByDefault>` (default true) |
| `Position Settings` | `X`, `Y`, `Width`, `Height` | `<position x y>`, `<size width height>` |

Children, each an ordered drawing instruction (persisted under `<display>`):

| Child | Stereotype | Properties | Persisted |
| --- | --- | --- | --- |
| `Path Draw Settings` | `Path Settings` | `Path`, `Condition`, `Line Color`, `Line Width`, `Line Dash Array`, `Fill Color` | `<drawPath>` |
| `Text Draw Settings` | `Text Settings` + `Position Settings` | `Text`, `Condition`, `Style` | `<drawText>` |
| `SVG Resource Draw Settings` | `SVG Resource Settings` + `Position Settings` | `Resource Path`, `Condition` | `<drawSvgResource>` |
| `Stereotypes Visual Settings` | `Position Settings` | where the «stereotype» line goes | `<stereotypesVisualSettings>` |
| `Element Visual Settings` | as above | a child row inside this shape | `<childElementVisualSettings>` |

`Resource Path` is a file name under the module's `resources/` folder (falling back to `content/`). Drawing instructions run in order, so later ones paint over earlier ones.

### Expressions

Values are JavaScript run by the client's sandbox: a body starting with `return`, `const`, `let` or `var` runs as code, and a value starting with a backtick is a template literal. Position and size accept `${…}` text: `<size width="${w}" height="18" />`, `<position x="${x}" y="${y + 8}" />`. A path is a backtick template that returns an SVG path: `` `M${x},${y} h${w} v${h} h-${w} z` ``. Available names:

| Name | Meaning |
| --- | --- |
| `x`, `y`, `w`, `h` | This element's position and size. |
| `getDimensions()` | `{ left, right, top, bottom, getTopLeft(), getTopRight(), getBottomLeft(), getBottomRight(), getCenterTop(), getCenterBottom(), getCenterLeft(), getCenterRight(), getCenter() }`, each point `{x, y}`. |
| `theme.isDark` | Pick colours per theme. |
| `element`, `visual` | The model element and its visual (`getPosition()`, `getSize()`, `isAutoResizeEnabled()`, …). |
| `elements` | This shape's own child rows: `elements.count`, and `elements['<Child Type>']` gives `{ visible, any(), count, size }` for the rows of that type. |
| `parent` | The parent shape's full context (`parent.x`, `parent.w`, `parent.elements[...]`); `null` for a top-level shape. |
| `$index`, `$firstOfType`, `$lastOfType` | A child row's position among siblings **of the same type only**; use `$firstOfType` in a separator's `Condition`. |

Colours return a CSS colour or `null` for the default. A `Condition` returns boolean and hides the instruction when false.

## Association Visual Settings

Stereotype `Setting` on `Association Visual Settings`:

| Property | Persisted | Notes |
| --- | --- | --- |
| `Line Type` | `<lineType>` | `Elbow Connector` (default) or `Curved`; persisted `Elbow-Connector` / `Curved`. |
| `Line Color`, `Line Width`, `Line Dash Array` | `<lineColor>` … | `Line Dash Array` is an SVG `stroke-dasharray`. |
| `Reverse Flow Direction` | `<flowDirectionReversed>` | Treat the association target → source when added to a diagram. |
| `Hide Label` | `<hideLabel>` | Newer Module Builders only. Leaves out the label users can otherwise set on the line. |

Each end (`Association Source End Visual Settings`, `Association Destination End Visual Settings`) takes:

| Stereotype | Properties | Persisted |
| --- | --- | --- |
| `Label Settings` | `Primary Label`, `Secondary Label` | `<primaryLabel>`, `<secondaryLabel>` (JavaScript returning text; the end's API is in scope) |
| `Point Settings` | `Path`, `Line Color`, `Line Width`, `Line Dash Array`, `Fill Color` | `<pointIndicator>`: the arrow head or diamond at the end |
| `Navigable Indicator Settings` | same | `<navigableIndicator>`: the marker for a navigable end |

`Path` is an SVG path definition.
