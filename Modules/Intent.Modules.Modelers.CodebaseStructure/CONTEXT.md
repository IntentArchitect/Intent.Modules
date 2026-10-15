# Intent.Modelers.CodebaseStructure — Context

## File classifications

- **One stereotype for folders and Template Outputs (1.2.0).** `File Classification` (an `Entries` item
  list of glob, review importance and classification; named `Custom File Classification` before 1.2.0)
  applies to Folder and Template Output (and other modules' element types, such as C# Project and Solution
  Folder) through the `Allows File Classification` trait, which the target filter checks with
  `element.hasTrait(...)`. The Root Folder package is the exception; see the next point.
- **The Root Folder is admitted by its type id, not the trait (1.2.0-pre.1).** Traits do not work on
  packages. Module Builder's Package Settings persistable has no `implements` list, so a trait on Root Folder
  never reaches the `.designer.settings`. The client's package script context also hardcodes
  `hasTrait: () => false` (`PackageTreeNode.ts`). The filter therefore also accepts
  `element.specializationId === "07e7b690-…"` (Root Folder). The user chose this over adding package-trait
  support to Module Builder and the client, because it works on clients already in use. The Root Folder still
  carries the trait; it is inert, but harmless, and removing it from the model would change the generated
  `RootFolderModel` API. The Visual Studio Solution element (Intent.VisualStudio.Projects) was not opted in.
- **On the Root Folder, globs are relative to the application's output location** (not shifted by
  `Root Folder Options → Relative Location`), consistent with how name-based folders map. The client's
  `FileClassificationIndex` adds the root's rows to the output root as the shallowest contributor, so any
  deeper folder or project wins and the root applies wherever nothing deeper matches. Like any folder rule,
  it never classifies a file that has a Template Output. Clients before that client change ignore root
  entries in Change Review, although the stereotype can still be applied.
- **On a Template Output, globs are relative to the folder containing it.** Module install (the client's
  `ApplicationDesignerPersistable`) stamps a single `**/*` entry from the template's `.imodspec`
  classification and review importance. Per-file globs are designer-only. The `.imodspec` deliberately stays one
  classification per template.
- **`Output Classification` was deleted in 1.2.0** (the user's decision), reversing the earlier plan to keep
  it. This is safe because the migration and the client match stereotypes by raw id, not via the definition.
  The client still reads legacy data as a fallback, and the `supportedClientVersions` floor stops older
  clients from stamping it.
- **The stereotype and the element type share the name `File Classification`.** The user accepted that
  deliberately; ids differ, so nothing resolves by name.
- **"Severity" became "Review Importance"** (same property id, same `0 - None` … `3 - High` literals). It is
  about how important it is to review a changed file. The `.imodspec` tag Module Builder writes moves to
  `<reviewImportance>`; the client falls back to `<severity>` for already-published modules.
- **Migration `1.2.0-pre.0`** converts each Template Output's `Output Classification` into a
  `File Classification` with one `**/*` entry. It skips elements that already have a File Classification, since a user put that there. `Down()` only reverses a lone `**/*` entry, so it never
  discards user-added rows.
- **The `Entries` property value is a label cache** (a JSON array of row labels). It must match the
  `File Classification Entry` display function exactly: `[Review Importance - names] glob, glob`, trimmed, and
  `[] glob` when there is no review importance or classification. The migration's `FormatEntryLabel` and the client's
  `FormatEntryLabel` in `ClassificationStereotypeConstants` both mirror it. Change all
  three together.
- **The ids in the migration are copied from the client's `ClassificationStereotypeConstants`**, and
  `Migration_01_02_00_Pre_00Tests.Ids_match_the_module_metadata` pins them.
- **`supportedClientVersions` floor is `5.4.0-beta.6`**, the first client that stamps the entries shape when
  this module is 1.2.0 or later. With an older client, install would keep writing `Output Classification`.

- **Documentation is split by where it is read.** The `File Classification` stereotype's comment says only
  what it does and where it applies, and is kept short. How rows combine (last match wins, catch-alls first,
  never set the `Entries` value directly) is the `Entries` hint. Each field's rules are on that field's hint
  in `File Classification Entry`. Rejected: one long stereotype comment, because it put row-level guidance
  away from the field being edited. AI agents read these hints as authoritative, so keep them precise.
- **The `File Classification` element type's comment disambiguates it from the stereotype** of the same name.
- **The `File Classification` element's icon is tinted by an Icon Function** (1.2.0-pre.1). The function
  rebuilds the tag SVG with the same vertical gradient as the default icon: a lighter tint at the top,
  `color-mix(… 65%, white)`, down to a deeper shade at the bottom, `color-mix(… 85%, black)`. The stops are
  computed from `Color`, with a `[data-theme="dark"]` rule from `Color (Dark Mode)`; each falls back to the
  other. With neither set, it uses the same name-derived colour
  as Change Review, so the icon always matches the pill. The static blue gradient `Icon` is only seen if the
  function fails. Values are checked against plain CSS colour syntax before being inlined. If the tag shape
  changes, update the static `Icon` and the path in the function together.
  - **The name-derived colour is duplicated from the client.** `deriveColourFromName` in the function
    mirrors `DeriveColourFromName` in the client's `FileClassificationService`: FNV-1a over the UTF-8 bytes of
    the trimmed, lower-cased name, giving `hsl(hash % 360, 58%, 45%)`. A designer script can't call the
    client, so change both together. The JS uses `Math.imul(...) >>> 0` to match C#'s unsigned 32-bit
    overflow. It reads UTF-8 bytes from `encodeURIComponent`, which is available in the sandbox where
    `TextEncoder` may not be.
  - **The data URI must be base64.** The client's icon renderer (`ntIcon.ts`) `atob`-decodes every
    `data:image/svg+xml;` source, so a `;utf8,` percent-encoded URI silently renders nothing. `btoa` isn't
    available in the designer-script `vm` sandbox, so the function base64-encodes the ASCII-only SVG itself.
  - **The SVG renders inline, so its `<style>` and ids are page-wide.** The CSS classes and the gradient id
    embed the element id, so that each classification's colours stay scoped to its own icon.

## Gotchas

- **`Api/IAllowsCustomFileClassificationModel.cs` is a hand-written compatibility shim. Do not delete it.** It is the
  trait interface's pre-1.2.0 name, which renaming the trait removed from the generated output. Modules compiled
  against older versions implement it; Intent.VisualStudio.Projects 4.1.10's `CSharpProjectNETModel` and
  `SolutionFolderModel` do. Without it they fail to load with a `TypeLoadException`, which aborts the install and
  the migration. It inherits `IAllowsFileClassificationModel`, so those types also count as the new trait. Renaming
  a trait or any other generated API type is a binary break for every module built against the old name, so check
  dependents (e.g. with `System.Reflection.Metadata` over cached module DLLs) before renaming.

- **Trait interface namespace mismatch.** Module Builder generates trait interfaces (`ApiTraitModel`) in the
  project folder namespace, `Intent.Modules.Modelers.CodebaseStructure.Api`. Element models (`ApiElementModel`)
  go in `Module Settings → API Namespace`, which defaults to `Intent.Modelers.CodebaseStructure.Api`, and
  reference trait interfaces by bare name. `Api/TemplateOutputModel.cs` therefore carries a hand-added
  `using Intent.Modules.Modelers.CodebaseStructure.Api;`, which survives regeneration. Any other element given
  a trait needs the same fix until Module Builder resolves trait types properly. Setting `<RootNamespace>`
  (as Modelers.Domain does) was rejected because it would move every namespace in the project.
- **`Intent.Architect.Persistence` 3.7.4** (needed for `StereotypePropertyPersistable.ItemList`) pulls
  `Intent.SoftwareFactory.SDK` ≥ 3.11.0 and `Intent.Persistence.SDK` ≥ 1.0.2. The csproj pins those floors to
  avoid NU1605.
- **`new StereotypePersistable()` leaves `Properties` null** in that package version. Initialise it
  explicitly, and null-guard it when reading deserialized metadata.
