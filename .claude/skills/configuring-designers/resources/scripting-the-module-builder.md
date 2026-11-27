# Scripting the Module Builder model

You configure a designer by editing the **Module Builder** designer of the module's application with `run_designer_script`. The `.designer.settings` XML is generated output; every other resource gives its tag only so you can check it afterwards.

## Orient before you write

1. `get_designer_rules` for the Module Builder designer, then `getDesignerSchema()`: the element types and the children each accepts (`!` needs a type reference, `¹` allows one).
2. `getStereotypeDefinitions()`: every stereotype with `applicableTo`, its property labels, allowed values and the author's hint after ` — `. The labels are what you pass to `setProperty`; use a listed value exactly.
3. Find the module's own `Designer Settings` (or `Package Settings`, `Element Settings`) element with `findElements` and add under it. Do not create a second `Designer Settings` for a designer that has one.

## Stereotype names repeat: the element decides

`Settings` is several different stereotypes. `ensureStereotype("Settings")` on an element applies the one that element accepts.

| Element | Stereotypes to set |
| --- | --- |
| `Element Settings` | `Settings`, `Type Reference Settings`, `AI Settings` |
| `Core Type` | `Settings` |
| `Element Extension` | `Extension Settings`, `Type Reference Extension Settings`, `AI Settings` |
| `Association Source End Settings`, `Association Destination End Settings` | `Settings` |
| `Association Source End Extension`, `Association Target End Extension` | `Association End Extension Settings` |
| `Element Visual Settings` | `Settings`, `Position Settings` |
| `Path Draw Settings`, `Text Draw Settings`, `SVG Resource Draw Settings`, `Stereotypes Visual Settings` | `Path Settings`, `Text Settings`, `SVG Resource Settings`, and `Position Settings` on the last three |
| `Association Visual Settings` | `Setting` (singular) |
| `Association Source/Destination End Visual Settings` | `Label Settings`, `Point Settings`, `Navigable Indicator Settings` |
| Every option under a `Context Menu` | `Option Settings` (the properties differ per option type) |
| `Run Script Option`, `Element Event Handler`, `Association Event Handler`, `Script` | `Script Settings` |
| `Suggestion` | `Settings` |
| `Package Settings` | `Package Settings` |
| `Designer Settings` | `Designer Settings` |

## Recipe: an element type that draws as a box

Stereotype names, labels and where each element may sit come from the Module Builder's metadata; the script itself has not been executed, so confirm each name against the schema first. `Element Visual Settings` goes under its own `Element Settings` (as the Domain module does) or under a `Diagram Settings` for visuals that belong to one diagram.

```js
const settings = findElements({ specialization: "Designer Settings", name: "My Designer" })[0];
const type = settings.addChild("Element Settings", "Step");
const typeSettings = type.ensureStereotype("Settings");
typeSettings.setProperty("Save Mode", "Default");

const visual = type.addChild("Element Visual Settings", "Step");   // `!`: needs a type reference
visual.setType("Step");                                              // the element type it draws
const position = visual.ensureStereotype("Position Settings");
position.setProperty("Width", "160");
position.setProperty("Height", "60");
const box = visual.addChild("Path Draw Settings", "Box");
const path = box.ensureStereotype("Path Settings");
path.setProperty("Path", "`M${x},${y} h${w} v${h} h-${w} z`");
path.setProperty("Fill Color", "return theme.isDark ? '#333' : '#fff';");
```

- Keep the handle `addChild` returns; do not look the element up again by the name you passed.
- A property that is a JavaScript function takes the **body as a string**. Follow the body rule in `functions-scripts-and-events.md`: a body that does not start with `return`, `const`, `let`, `var`, `if` or `function` is read as a template literal.
- `ref to <Type>` properties take the **id** of an element of that type, and `(multiple)` takes an array of ids. Resolve them by name in the same script.
- Item lists (such as a stereotype's row list) use `prop.addItem()`, not `setValue`; see `scripting-the-model`.

## Verify

Read `errors[]` from the script result and fix what you introduced. Then run the Software Factory and read the generated `modelers/*.designer.settings` for the element you changed: the other resources give the tag each setting lands in. A setting that never appears there usually means the property was set on the wrong stereotype.

## If the property is not in the schema

The module's installed Module Builder may predate the setting (the schema, not these resources, is the authority). Do not add it to the generated XML. Tell the user the Module Builder module needs updating, and ask them to do that first.
