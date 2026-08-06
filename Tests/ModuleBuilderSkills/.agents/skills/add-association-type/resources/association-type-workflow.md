---
contentHash: D625585E86A3B7ACD7D2A66D52AE876483838E3955EA2E568FE71C17BB7700A2
---
# Add Association Type — Workflow & Details

## Workflow

### Step 1 — Identify Source and Target Element Type IDs

You need the `typeId` of every element type that either end of the association can connect to. Collect these before making any model changes.

- *MCP lookup per type:**

```json
find_designer_elements(
  applicationId   = <target module's app ID>,
  designerId      = <target module's Module Builder designer ID>,
  query           = "<element name>",
  fields          = ["name"],
  specializations = ["Element Settings"]
)
```

Or read `SpecializationTypeId` from the generated `Api/<TypeName>Model.cs` in the target module.

### Step 2 — Find the Right Designer Settings Node

The `Association Settings` element must live under the correct `Designer Settings` node — the one that carries the `Extend Designers` GUID of the designer where this association will be visible.

If the node doesn't exist, create one and set `Extend Designers` to the appropriate designer GUID:

- Domain: `6ab29b31-27af-4f56-a67c-986d82097d63`
- Services: `81104ae6-2bc5-4bae-b05a-f987b0372d81`

### Step 3 — Create the Association Settings Element

Create an `Association Settings` element under the `Designer Settings` node. Name it after the association (e.g., `Publish Integration Event`).

It will automatically get two children: `Association Source End Settings` and `Association Destination End Settings`.

### Step 4 — Configure the Source End

Select the `Association Source End Settings` child and apply the `Settings` stereotype. Key properties:

| Property | Value / Notes |
|---|---|
| `Target Types` | Array of type GUIDs this end can originate from (e.g., Command, Query, Operation) |
| `Api Property Name` | C# property name on the generated source-end model (e.g., `IntegrationEventsSources`) |
| `Display Text Function` | JS returning display text — usually `return typeReference.display` or a custom label |
| `Name Accessibility` | Usually `Hidden` (the name is not user-editable) |
| `Name Must Be Unique` | `true` / `false` |
| `Allow Multiple` | Whether multiple of this association type can originate from the same element |
| `Is Navigable Default` | `false` for source end on publish-style associations |

### Step 5 — Configure the Target End

Select the `Association Destination End Settings` child and apply the `Settings` stereotype. Key properties:

| Property | Value / Notes |
|---|---|
| `Target Types` | Array of type GUIDs this end can connect to (the target element type) |
| `Api Property Name` | C# property name on the generated target-end model (e.g., `PublishedIntegrationEvents`) |
| `Display Text Function` | JS — often uses keyword prefix: `return [{ text: "[publish] ", cssClass: "keyword" }].concat(typeReference.getDisplayTextComponents())` |
| `Name Accessibility` | Usually `Hidden` |
| `Is Navigable Default` | `true` for target end (navigable from the source) |
| `Allow Multiple` | Whether multiple target-end connections are permitted |
| `Allow Sorting` | `true` if ordering matters |

- *Applying traits:**

If this association end has a semantic role (e.g., it is a "processing action" that can be mapped), apply a trait stereotype:

- `[Processing Action]` — the target end represents something that sends/publishes
- `[Processing Handler]` — the target end represents something that handles/receives

Traits are how mapping settings and runtime template code identify which ends are mappable.

- *Adding a Mapping Option to the target end:**

If users should be able to open a mapping dialog from this association end:

1. Create a `[context menu]` child under the `Association Destination End Settings`.
2. Add a `Mapping Option` under it.
3. Set its type reference to your module's `Mapping Settings` definition.
4. Apply `Option Settings` with a `Shortcut` (e.g., `ctrl + space`).

### Step 6 — Configure Visual Settings (Optional)

Create an `Association Visual Settings` child under the `Association Settings` element. Apply the `Setting` stereotype:

- `Line Type` — `Curved`, `Elbow Connector`, `Straight`
- `Line Color` — JavaScript returning a CSS color string (can be theme-aware)
- `Line Dash Array` — e.g., `"3,7"` for a dashed line
- `Reverse Flow Direction` — `true` for subscribe/handler associations (arrow points toward the handler)

### Step 7 — Expose It via an Extension

A bare `Association Settings` definition is only part of the work — users also need a way to *create* it. Wire it up via an element or package extension:

1. Find or create the `Element Extension` for the source element type (e.g., `Command Extension`).
2. Add an `Association Creation Option` to its `[context menu]`.
3. Set the option's type reference to the **target end** of your new association (`Association Destination End Settings` child's ID).
4. Apply `Option Settings` (`Allow Multiple`, `Menu Group`, `Shortcut`).

See the `add-designer-extension` skill for the full element extension workflow.

### Step 8 — Run Software Factory

```powershell
run_software_factory(applicationId = <your module's app ID>)
```

Review the diff. You should see:

- A new `<elementSettings>` block (or `<associationSettings>` block) in `modelers/*.designer.settings`
- A new `Api/<AssociationName>Model.cs` with source/target end models
- A new `Api/<AssociationName>ModelAssociationExtensions.cs`
- Updated `Api/ApiMetadataProviderExtensions.cs`

```powershell
apply_staged_file_changes(applicationId = <your module's app ID>)
```

### Step 9 — Verify

1. `get_designer_validation_errors` — resolve any errors.
2. Confirm the generated `*Model.cs` has `SpecializationTypeId` matching the element's GUID.
3. Confirm `SourceEnd` and `TargetEnd` model properties exist with the correct `ApiPropertyName` values.
4. Build the module project to confirm compilation.
- --

## Common Mistakes

- **Swapping source and target end Target Types** — the source end's `Target Types` are the element types the association *starts from*; the target end's are the types it *connects to*. Getting these backwards means the association appears on the wrong element's context menu.
- **Setting `Is Navigable Default = true` on both ends** — for directed associations (publish, subscribe), only the target end should be navigable by default.
- **Forgetting to wire up the creation option** — defining `Association Settings` alone doesn't put anything in the UI. Users need an `Association Creation Option` on a package/element extension to actually draw it.
- **Putting the Mapping Option on the source end** — mapping options almost always belong on the **target end** since that's the action end (what is being published/sent/invoked).
- **Not applying traits when mappings need them** — if your module's `Mapping Settings` expects `[Processing Action]` or `[Processing Handler]` traits to identify mappable ends, these stereotypes must be applied to the association end — they don't come automatically from the association type name.
- **Editing `.designer.settings` directly** — regenerated on every SF run. Always change the Module Builder model.
