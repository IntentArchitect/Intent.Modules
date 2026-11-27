# Diagram anchors

Where a curved association meets an element: it takes the segment whose middle is **nearest the other end**. A lone line sits on that middle point. With spreading on, lines sharing a segment are spread evenly along it: the segment is divided between them, never closer than 20px, and when it cannot fit them they bunch at its ends. A self-association never spreads. Straight and elbow lines ignore anchors.

## Settings

Properties of the `Settings` stereotype on the element type's `Element Visual Settings` (a nested row uses the same element type). Set them as in `scripting-the-module-builder.md`.

| Property | Value | Effect | Generated tag |
| --- | --- | --- | --- |
| `Anchor Segments` | Function body returning `[[from, to], …]` | Stretches of outline lines attach to; a zero-length pair `[p, p]` is a single point. Works with or without spreading. | `<anchorSegmentsFunction>` |
| `Anchor Spread` | `Off` (default) or `Even` | `Even` spreads lines sharing a segment evenly along it. `Off` emits nothing and leaves diagrams unchanged. | `<anchorSpread>even</anchorSpread>` |
| `Anchor Points` | Function body returning `[point, …]` | Deprecated: use `Anchor Segments`. Shown only while it already has a value, and still written for clients that predate segments. Segments replace it when both are set. | `<anchorPointsFunction>` |

Function bodies use `getDimensions()` (absolute diagram coordinates, not offsets). A function that throws, or returns an empty array, is ignored and the defaults apply: the four edge midpoints.

## Check the schema first

Older Module Builders have only `Anchor Points`, and they cannot spread. Look for `Anchor Segments` and `Anchor Spread` in `getStereotypeDefinitions()` (stereotype `Settings`, applicable to `Element Visual Settings`) before promising them. If they are missing, do not edit the generated `.designer.settings`: the next Software Factory run overwrites it. Tell the user the module's Module Builder needs updating, and ask them to update it first.

## Configurations

Each shows the property values to set. Configs 5 to 7 are untried: check them on a diagram.

### 1. Ordinary box: spread on every side
`Anchor Spread` = `Even`. No function needed: the four edge midpoints are the default, and each stands for its whole edge.

### 2. Left and right only
`Anchor Spread` = `Even`. `Anchor Segments`:
```js
const d = getDimensions(); return [
  [{x: d.left, y: d.top}, {x: d.left, y: d.bottom}],
  [{x: d.right, y: d.top}, {x: d.right, y: d.bottom}]];
```
On a short element (an 18px row) the 20px minimum gap makes lines bunch.

### 3. Top and bottom only (flow-chart step)
`Anchor Spread` = `Even`. `Anchor Segments`:
```js
const d = getDimensions(); return [
  [{x: d.left, y: d.top}, {x: d.right, y: d.top}],
  [{x: d.left, y: d.bottom}, {x: d.right, y: d.bottom}]];
```

### 4. Diamond (decision)
`Anchor Spread` = `Even`. `Anchor Segments`:
```js
const d = getDimensions(); return [
  [d.getCenterTop(), d.getCenterRight()], [d.getCenterRight(), d.getCenterBottom()],
  [d.getCenterBottom(), d.getCenterLeft()], [d.getCenterLeft(), d.getCenterTop()]];
```

### 5. Hexagon
`Anchor Spread` = `Even`. `Anchor Segments`:
```js
const d = getDimensions(), w = d.right - d.left, m = (d.top + d.bottom) / 2;
const a = {x: d.left + w * 0.25, y: d.top}, b = {x: d.right - w * 0.25, y: d.top}, c = {x: d.right, y: m},
      e = {x: d.right - w * 0.25, y: d.bottom}, f = {x: d.left + w * 0.25, y: d.bottom}, g = {x: d.left, y: m};
return [[a, b], [b, c], [c, e], [e, f], [f, g], [g, a]];
```
A curved shape such as an ellipse is approximated by a polygon of straight sides like this.

### 6. Rounded box: keep lines off the corners
`Anchor Spread` = `Even`. `Anchor Segments`:
```js
const d = getDimensions(), r = 14; return [
  [{x: d.left + r, y: d.top}, {x: d.right - r, y: d.top}], [{x: d.left + r, y: d.bottom}, {x: d.right - r, y: d.bottom}],
  [{x: d.left, y: d.top + r}, {x: d.left, y: d.bottom - r}], [{x: d.right, y: d.top + r}, {x: d.right, y: d.bottom - r}]];
```

### 7. Class with a header: sides start below the title, lines from above share one point
`Anchor Spread` = `Even`. `Anchor Segments`:
```js
const d = getDimensions(), h = 36; return [
  [{x: d.left, y: d.top + h}, {x: d.left, y: d.bottom}], [{x: d.right, y: d.top + h}, {x: d.right, y: d.bottom}],
  [d.getCenterTop(), d.getCenterTop()]];
```

### 8. No spreading (today's behaviour)
`Anchor Spread` = `Off`. Lines end on the middle of the nearest segment from `Anchor Segments`, or on the nearest of the four edge midpoints when it is unset. Use `[p, p]` segments to pin lines to exact points.

## Gotchas

- Only curved associations use anchors or spread. Check the association's `Line Type` is `Curved` (see `diagram-visuals.md`) before concluding the setting does nothing.
- After the Software Factory runs, the generated `<visualSettings>` for the type should hold the tags in the table; a missing tag means the property went on the wrong element.
- To verify visually: open a diagram where two or more curved lines reach the same side of an element of this type, and check their ends are spaced apart rather than meeting at one point.
