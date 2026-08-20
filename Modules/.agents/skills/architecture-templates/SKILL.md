---
name: architecture-templates
description: "Build or extend an Architecture Template — the component/module picker shown when creating a new application — and its companion metadata application that pre-seeds new applications with designer content (packages, folders, Output Anchors). USE ONLY WHEN asked to create, extend, or fix an architecture template, or to seed default designer metadata for one. DO NOT USE FOR a 'Module Building' template (a different Template Type that scaffolds a module project, not an app) or day-to-day module development itself (see module-building-strategies). REQUIRES the Intent Application Template Builder designer, in a new Package."
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ArchitectureTemplates_SkillMd_Agents
contentHash: FDA2BE13598EB16A593075E55855365851BBDFB7023A45800D0B21A56696A68D
---
# Architecture Templates

An Architecture Template ("Application Template" is the older, still-used name for the same thing) is the menu of choices shown when creating a new Intent Architect application — it decides which modules get installed, nothing more. Getting a *new* application's designers pre-populated with actual content (packages, folders, Output Anchors) is a **separate** mechanism — a companion metadata application whose designer content gets exported and installed alongside the template. Both parts are covered here.

## 1. Building the template itself

Create the template using the **Intent Application Template Builder** designer, in a new Package.

### Application Template (root element)

Carries two stereotypes:

- **Application Template Settings** — `Template Type` (`Architecture Template` or `Module Building` — the latter scaffolds a *module project*, not an app; use `Architecture Template` for this workflow), `Icon`, `Version`, `Display Name`, `Images`, `Short Description`, `Long Description`, `Authors`, `Priority` (higher shows first in the picker), `Supported Client Versions` (interval notation — see below).
- **Application Template Defaults** — default application `Name`, `Relative Output Location` (e.g. `${application.name}`), solution/folder layout toggles, and whether to write `.gitignore` entries.

### Component Group → Component → Component Module

- **Component Group** — organizes related choices (e.g. "Persistence", "Security"). `Selection Mode`: `Allow Multiple` or `Allow Single Only`.
- **Component** — one selectable capability. `Icon`, `Description`, `Include by Default`, `Is Required`, `Dependencies`/`Incompatibilities` (refs to other Components), `Required License` (None/Professional/Premium), `Documentation Url`, `Tags`.
- **Component Module** — the actual module(s) a Component installs. The element's **name must be the exact module id** (e.g. `Intent.Modelers.Domain`) and its `Version` property must be a version that module actually publishes — get both from `search_available_modules`, never guess. Other properties: `Include By Default`, `Is Required`, `Include Assets` (`All`/`None`/`Select` — when `Select`, choose from `Application Settings`, `Designer Metadata`, `Designers`, `Factory Extensions`, `Template Outputs`).

### Settings Configurations (user-facing options at app-creation time)

- **Settings Configuration** — one "heading" of related fields; a template can have several. A configuration with no fields, or where every field is `Hidden`, doesn't show in the wizard at all.
- **Field Configuration** — one input. `Name` is the wizard label; `Value` is the logical name other files reference it by (`${<value>}` — see template strings below). `Control Type`: Text Box / Number / Checkbox / Switch / Text Area / Select / Multi-Select / Hidden. `Is Required`, `Hint`, `Default Value`.
- **Field Option** — the choices for a `Select`/`Multi-Select` field; `Name` is the display label, `Value` is what gets substituted.
- **Driving a Module Setting from the wizard**: give a Field Configuration the exact `id` of the target Module Setting field as its `Value` (find that id by installing the module once, then searching its label in the app's `.application.config` file). Since Module Settings can still be changed after creation, it's common to make these `Hidden` with just a `Default Value`.

### Generate and package

Run the Software Factory — this produces the package's `metadata.iatspec` file and packages the whole thing into a `.iat` file (the same role `.imodspec`/`.imod` play for a plain module). Test it by adding the output folder as a repository in the Repository Manager, then creating a new application from it.

## 2. Seeding default designer content (the companion metadata app)

The template above only decides *which modules* install. To make a freshly created app start with actual packages/folders/diagram content already in place, model that separately and install it as metadata — **never** by exporting from a real, working application.

### Workflow

1. In your Modules-style solution, create a **new, otherwise-empty application** named `<TemplateName>.Metadata` (e.g. `MyOrg.ApplicationTemplates.Metadata`) — press "Create Empty", confirm the empty-app prompt.
2. Install **only** the module(s) that own the designer(s) you want to seed (e.g. `Intent.VisualStudio.Projects` for the Visual Studio / Codebase Structure designer). Don't install the whole template's module set — you're modeling structure, not generating real code.
3. Open the relevant designer, create a package, and model the desired starting state using **template strings** directly as element names — `${application.name}`, `${solution.name}`, etc. (see the reference table below). Using these instead of literal names means the exported metadata needs no manual "correction" afterward.
4. Press the designer's own **Export** button (toolbar) — copies an XML representation of everything in the designer (even unsaved content) to the clipboard. **There is currently no MCP-tool equivalent of this** — it's a manual, UI-only action, so ask the user to do this step and paste you the result rather than trying to script around it.
5. Paste the clipboard content into a new file:
  - Application Template → `resources/<designer-name>.installation.config`
  - Plain Module → `content/<designer-name>.installation.config` (different folder — easy to get wrong)

   Name the file after the designer it targets (`domain.installation.config`, `services.installation.config`, `visual-studio-projects.installation.config`, …) — this is convention, not enforced, but every real template follows it.

6. Rebuild. Confirm the build log shows the file being packaged (`Added resources/....installation.config` / `Added content/....installation.config`).

### Metadata merging (what happens on install)

Intent Architect avoids duplicating elements: for each element in the `.installation.config`, it first tries to match an existing element by `id`/`externalReference`, then falls back to matching by name + folder path; on a match it merges child elements recursively using the same logic. Every installed element gets a **new** id, with the source id preserved in `externalReference` so this correlation keeps working across installs.

- *For a plain Module** (not an architecture template), metadata installation only happens on **first install** — never on upgrade or reinstall — so a user who deleted the seeded metadata doesn't get it forced back.

### What not to export

Don't export/install a designer's **Template Output** elements — those are module-managed (created/removed/renamed automatically as modules install/update) and should be left to Output Anchors (below) instead of being captured as static metadata.

## 3. Output Anchors — controlling where generated content lands

This is the same mechanism module-building already calls a template's **Role** — "Anchor" is just the Codebase-Structure-designer-side name for it, chosen to read intuitively to end users ("an area where templates get installed into"). A File Template's `Role` property and a Codebase Structure `Output Anchor`'s name are the same string, matched exactly.

- **Anchors are arbitrary tags** — there's no canonical registry. A module author picks a Role name; an architecture template author places an Output Anchor with that same name somewhere in Codebase Structure. They only need to agree on the string.
- **De facto convention**: `AI.Context` / `AI.Context.Skills` / `AI.Context.Instructions` is already present in essentially every current .NET architecture template (Clean Architecture, both Blazor variants, both Modular Monolith variants, Windows Service Host, AWS Lambda, Azure Functions) and even the Angular web client template — reuse this prefix for any new AI-context content rather than inventing a new one.
- **Matching walks the dotted segments, most-specific first.** A Role of `Domain.Entity` targets an anchor named exactly `Domain.Entity` if one exists; otherwise it falls back to `Domain`; and so on up the chain. This is why naming a new, more specific Role under an existing dotted prefix (e.g. `AI.Context.*`) is safe — it degrades gracefully to the shared parent anchor on architecture templates that haven't been updated with the more specific one.
- **A Role can list several unrelated candidates, comma-separated**, tried in order — for bridging a genuinely different naming convention (e.g. an older architecture template that never adopted a given prefix). This is a different mechanism from the automatic dot-segment walk-up above; reach for it only when the fallback name isn't just a prefix truncation of the first.
- **Anchors only control *initial* placement.** Once a Template Output is placed, moving it in Codebase Structure (or leaving it where a user moved it) survives module updates/reinstalls — the anchor is consulted once, not on every regeneration. To force re-placement you'd have to uninstall and reinstall the module.
- Roles also serve a **second, unrelated purpose**: looking up a specific template *instance* in factory-extension code (`TemplateRoles.*` constants, `IAppStartupTemplate.RoleName`) is preferred over hardcoding a `TemplateId`. Same naming mechanism, different axis (a template instance's identity vs. a Codebase Structure placement point) — see `intent-module-orchestrator` for the lookup APIs.

## 4. Template string reference

Available in `.installation.config` files and in File Installation Rule content (below):

| Template string | Resolves to |
|---|---|
| `${solution.name}` | The Intent Architect Solution name |
| `${application.name}` | The Intent Architect Application name |
| `${mcpServer.executable}` | Path to the Intent Architect MCP server executable |
| `${mcpServer.executableArguments}` | Arguments to pass to the MCP server executable |
| `${<field-configuration-value>}` | Any Settings Configuration Field Configuration's captured (or default) value |

Wrap any of the above in a case-conversion function: `camelCase(...)`, `pascalCase(...)`, `kebabCase(...)` — e.g. `${kebabCase(application.name)}`.

## 5. File Installation Rules (copying whole files)

For files that should just be copied verbatim (not designer metadata) — configured on the Application Template's `[installation settings]` node:

| Property | Meaning |
|---|---|
| `Name` | Display name for the rule (typically the filename) |
| `Match Files` | Filename or glob pattern, matched against files in the template's `resources` folder |
| `Target` | `Output Directory` (the created app's generated-code root) or `Application Config Directory` (the Intent Architect config folder) |
| `Relative Output Folder` | *(optional)* subfolder within the target |

File content also supports the template-string substitution above.

## Verification checklist

- [ ] The Component Module's name is the module's exact id, and its Version is a real, published version — confirmed via `search_available_modules`, not guessed
- [ ] `Supported Client Versions` uses interval notation (`[a,b)` — inclusive lower, exclusive upper)
- [ ] Any seeded metadata was exported from a dedicated `<Name>.Metadata` app — never a production application
- [ ] `.installation.config` files live in `resources/` (Application Template) or `content/` (Module) — not the other folder
- [ ] No `Template Output` elements were captured in exported metadata
- [ ] Any new Role/Anchor name shares the `AI.Context` (or other existing) dotted prefix where one already fits, rather than inventing an unrelated tag
- [ ] Element/package names in seeded metadata use template strings (`${application.name}`, etc.), not literal names
