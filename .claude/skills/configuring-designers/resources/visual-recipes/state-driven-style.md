# State-driven style

Make colour or icon depend on the element's data, so the diagram says something without extra shapes. All three below are in the Domain `Class` and its rows. Put them on the instruction they change; each value is a function body that starts with `return` (or `if`/`let`).

## Outline for external or referenced elements

`Path Draw Settings` → `Line Color`:

```js
if (element.isReference()) { return theme.isDark ? '#FFB312' : '#00A2E8'; }
return null;
```

`null` keeps the default outline. A referenced element comes from another package, so it gets a different outline from your own.

## Fill from a relationship (aggregate root)

`Path Draw Settings` → `Fill Color`:

```js
const isAggregateRoot = !element.getAssociations("Association")
  .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
if (isAggregateRoot) { return theme.isDark ? '#363d4d' : null; }
return theme.isDark ? '#1e2a38' : '#edf4fb';
```

The pattern is: read the element's associations, stereotypes or properties, then return one palette pair per case. `getAssociations(type)` gives the associations of that type; each has `isSourceEnd()` and a `typeReference`.

## Conditional icon

Two `SVG Resource Draw Settings`, each with a `Condition`, at the same position:

| `Condition` | `Resource Path` |
| --- | --- |
| `return !element.getIsStatic();` | `operation.svg` |
| `return element.getIsStatic();` | `operation-static.svg` |

Only the one whose condition is true is drawn. Use this for any binary marker (static, abstract, required).

