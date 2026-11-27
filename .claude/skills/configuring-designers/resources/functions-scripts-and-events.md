# Functions, scripts, event handlers and suggestions

## Function contracts

Many settings are a **JavaScript function body** that returns a value. Start it with `return`, `const`, `let`, `var`, `if` or `function`. The client rewrites any other body into a template literal, so a bare `element.isReference()` returns its own source text and is always truthy. Only a body that starts with a backtick, or is an `a ? b : c` expression, is used as is. A function that throws is skipped with a warning, so test in the app.

| Setting | Returns |
| --- | --- |
| `Display Text Function` | string, or `[{ text, cssClass?, color?, targetId?, markdown? }]` |
| `Validate Function` | string or `{ text, type?: "error"\|"warning" }`; `null` means valid |
| `Icon Function` | icon string, or `null` for the default |
| `Is Option Visible Function`, `Condition`, `Filter Function` | boolean |
| `Default Name Function` | string |
| `Anchor Segments` | `[[from, to], …]` of `{x,y}` (see `diagram-anchors.md`); the deprecated `Anchor Points` returns `{x,y}[]` |

Globals depend on where the function runs: an element function sees `element` (the model element), `application`, `getAssociations(typeId)` and `theme`; an association end function also sees `typeReference` and `getOtherEnd()`; a diagram drawing function adds `x`, `y`, `w`, `h`, `getDimensions()` and `visual` (see `diagram-visuals.md`). The full lists are the `*.context.api.d.ts` files under `wwwroot/App/Api/TypeDefinitions` in the client.

## Scripts

`Script` elements under `Designer Settings` are reusable code. `Script Settings` stereotype: `Type` (`Inline Script` or `File Resource`), `Script`, `File Path` (relative to the module's `resources/` folder), `Dependencies` (other `Script`s). Reference one from a handler or run option instead of copying it. Persisted as `<scripts>/<script name id>`.

## Element, package and association event handlers

`Element Event Settings` (on element types and extensions), `Package Event Settings` (on package types), `Association Event Settings` (on association types) each hold `Element Event Handler` / `Association Event Handler` children. Each handler has a required type reference and, when that is an `Inline Script`, `Script Settings` (`Script`, `Dependencies`: other `Script` elements). How the trigger is chosen in the model is not verified here: inspect an existing handler with `run_designer_query` before creating one. Persisted as `<macros>/<macro trigger>`.

| Trigger | Fires | Element | Association | Package |
| --- | --- | --- | --- | --- |
| `on-loaded` | the model loads | ✓ | ✓ | ✓ |
| `on-created` | it is created | ✓ | ✓ | ✓ |
| `on-changed` | any property changes | ✓ | ✓ | ✓ |
| `on-name-changed` | renamed | ✓ | ✓ | |
| `on-type-changed` | its type reference changes | ✓ | ✓ | |
| `on-deleted` | it is deleted | ✓ | ✓ | |
| `on-mapped` | a mapping to it changes | ✓ | | |

Rules:

- A handler that edits the element retriggers `on-changed`. Guard it so it settles.
- Element `on-created` and `on-name-changed`, and association `on-created`, run synchronously; keep those short.
- Stereotypes have their own triggers (`on-applied`, `on-changed`, `on-removed`) defined on the stereotype definition, not here.

## Suggestions

`Suggestions Settings` (one per `Designer Settings`) holds `Suggestion` children that propose actions to the user. `Settings` stereotype:

| Property | Persisted | Meaning |
| --- | --- | --- |
| `Locations`: `Diagram`, `Model` | `locations` | Where it shows. |
| `Filter Function` | `<filterFunction>` | Boolean: offer it for this element? |
| `Display Function` | `<displayFunction>` | Text shown. |
| `Icon` | `<icon>` | |
| `Script`, `Dependencies` | `<script>` | What accepting it does. |
| `Order Priority` | `<orderPriority>` | Lower sorts first; ties by title. |

Each is persisted as `<suggestionSetting type typeId>`.

## Run Script Option

A `Run Script Option` in a context menu uses `Script Settings` the same way; see `context-menus.md`.
