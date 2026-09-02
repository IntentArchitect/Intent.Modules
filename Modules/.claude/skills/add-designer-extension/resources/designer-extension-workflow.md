---
contentHash: 2933927CAAC2A485C83F8C02FE5708852B9734411F779F2A73227EA1558AB566
---
# Add Designer Extension — Workflow & Details

## When to Use Which Extension Type

| You want to add options to... | Use |
|---|---|
| A **package** (top-level container) | `Package Extension` |
| An **element** (Class, Command, Query, etc.) | `Element Extension` |
| The end of an existing **association** | `Association Extension` — target or source end extension child |

===

## Workflow

### Step 1 — Identify the Target Type ID

You need the `typeId` of the element or package you are extending. Never guess.

- *Option A — MCP lookup:**

```json
find_designer_elements(
  applicationId = <target module's application ID>,
  designerId    = <target module's Module Builder designer ID>,
  query         = "<element name>",
  fields        = ["name"],
  specializations = ["Element Settings"]  // or "Package Settings"
)
```

The result's `id` field is the typeId.

- *Option B — Generated C# constant:**

Open `Api/<TypeName>Model.cs` in the target module and read `SpecializationTypeId`.

### Step 2 — Find or Create the Designer Settings Node

In your module's Module Builder designer, find the `Designer Settings` node that extends the foreign designer. If it doesn't exist:

1. Create a `Designer Settings` node under your module's `designers/` folder.
2. Set its `Extend Designers` field to the foreign designer's GUID.
  - Domain Designer: `6ab29b31-27af-4f56-a67c-986d82097d63`
  - Services Designer: `81104ae6-2bc5-4bae-b05a-f987b0372d81`

### Step 3 — Create the Extension Element

- *For a Package Extension:**

Create a `Package Extension` element under the `Designer Settings` node. Set its type reference to the foreign package type ID found in Step 1.

- *For an Element Extension:**

Create an `Element Extension` element under the `Designer Settings` node. Set its type reference to the foreign element type ID found in Step 1.

Apply two stereotypes (required):

- `Type Reference Extension Settings` with `Mode = Inherit`
- `Extension Settings`
- *For an Association Extension:**

Create an `Association Extension` element. Under it, create either `Association Source End Extension` or `Association Target End Extension` children (whichever end needs the menu). Apply `Association End Extension Settings` to each end.

### Step 4 — Add the Context Menu Option

Under the extension element (or under the association end extension), create a `[context menu]` child, then under it:

- *Element Creation Option** — when the option creates a child element:
- Set type reference to the new element's specialization type ID
- Apply `Option Settings` stereotype:
  - `Default Name` — e.g., `NewMyElement`
  - `Allow Multiple` — `true` / `false`
  - `Shortcut` — e.g., `ctrl + shift + m` (optional)
  - `Menu Group` — integer; same number = visually grouped together
- *Association Creation Option** — when the option draws an association:
- Set type reference to the **target end** of the new association type
- Apply `Option Settings` stereotype with same fields as above
- *Mapping Option** — when the option opens a mapping dialog:
- Set type reference to the `Mapping Settings` definition
- Apply `Option Settings` with `Shortcut`

### Step 5 — Set Type Order (Optional)

If you have multiple new types appearing inside a package, add a `Type Order` child to the extension element listing each type in the desired display order.

### Step 6 — Run Software Factory

```powershell
run_software_factory(applicationId = <your module's app ID>)
```

Review the diff — you should see your extension appear in `modelers/*.designer.settings` inside a `<packageExtensions>` or `<elementExtensions>` block.

```powershell
apply_staged_file_changes(applicationId = <your module's app ID>)
```

### Step 7 — Verify

1. Check `get_designer_validation_errors` — resolve any errors before continuing.
2. Confirm the `.designer.settings` file shows the new `<packageExtension>` or `<elementExtension>` block with the correct `typeId`.
3. If the extension adds a new element type (not just a menu option to an existing one), verify `Api/<NewType>Model.cs` was generated with the correct `SpecializationTypeId`.

===

## Common Mistakes

- **Wrong typeId** — using the extension's own ID instead of the foreign type's ID in the type reference. The type reference on an `Element Extension` must point to the element you are *extending*, not to the element you are *creating*.
- **Missing `Extend Designers`** — creating the extension elements without linking the `Designer Settings` node to the foreign designer. The extensions are invisible until this GUID is set.
- **Skipping `Type Reference Extension Settings` stereotype** — without `Mode = Inherit` on an Element Extension, type reference behaviour is undefined and the designer may misbehave.
- **Adding context menu to the extension directly instead of the `[context menu]` child** — the menu options must live inside a `[context menu]` element, not directly on the extension element.
- **Editing `.designer.settings` directly** — this file is regenerated on every SF run. Always make changes in the Module Builder designer model.
