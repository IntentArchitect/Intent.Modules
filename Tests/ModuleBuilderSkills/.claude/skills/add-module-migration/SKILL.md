---
name: add-module-migration
description: "Add a Version Migration (or On-Install/On-Uninstall Migration) that programmatically edits an already-installed consumer application's own persisted metadata during install/update. USE ONLY WHEN a module's own restructuring, rename, or model/stereotype shape change needs applications that installed an earlier version to converge onto the shape a fresh install already gets. DO NOT USE FOR generating code into a consumer via the module's own templates (see file-builder-expert) or for bumping the module's own version number (see module-version-increment). REQUIRES the target module's Module Builder designer already open, and its version already incremented for the change being migrated."
template-id: Intent.ModuleBuilder.AI.Skills.Skills.AddModuleMigration_SkillMd_Agents
keywords: [migration, version-migration, on-install, on-uninstall, persistence, consumer-metadata, module-update]
contentHash: 1D6929DD25E0DD7B981FE14F19149C0629151E86574EEB17799AB9CD6E59D2E1
---
# Skill: add-module-migration

## What A Migration Is For

A module's own File Templates, stereotypes, or settings can change shape between versions. A brand-new
install of the module gets the new shape for free. An application that installed an earlier version does
not — its own persisted metadata (packages, folders, elements) still reflects whatever the old version
left behind, and nothing regenerates it automatically. A **Module Migration** is code that runs during
install/update and edits that consumer's own metadata directly, so it converges onto the shape a fresh
install already has.

## Modeled As An Element, Not Hand-Written From Scratch

Under a `Module Migrations` container element (one per module — create it if missing) you add:

- **`Version Migration`** — runs once, the first time a consumer updates across the version it targets.

Add as many as the module has migrated versions.

- **`On-Install Migration`** — runs once, on every fresh install (at most one).
- **`On-Uninstall Migration`** — runs once, on uninstall (at most one).
- *The `Version Migration` element's name must be the exact semver string it targets** — e.g. `1.0.1-pre.3`,

not a descriptive name like `FlattenSkillsFolders`. Confirmed against a real shipped module's Migrations
tree (`Intent.Modules.VisualStudio.Projects`): `On-Install`, `3.8.10-pre.2`, `3.9.2-pre.0`, `4.0.0-pre.0`,
… one entry per version that ever needed a migration. The generated class name derives directly from the
element name (dots/hyphens become underscores — `1.0.1-pre.3` → `Migration_01_00_01_Pre_03`), and its
`ModuleVersion` property is set to that same string — both `[IntentFully]`, never hand-edit them.

- *Increment the module's version first, then create the Version Migration element.** The Software Factory

captures whatever the module's current version is at generation time into the element's generated class -
creating the migration before bumping the version bakes in the wrong version.

## What Gets Generated, What You Add

Running the Software Factory after creating the element produces a class implementing `IModuleMigration`
(`Intent.Plugins`, from the `Intent.SoftwareFactory.SDK` package every module already references):

```csharp
public class Migration_01_00_01_Pre_03 : IModuleMigration
{
    public Migration_01_00_01_Pre_03()
    {
    }

    [IntentFully]
    public string ModuleId => "...";
    [IntentFully]
    public string ModuleVersion => "1.0.1-pre.3";

    public void Up() { }
    public void Down() { }
}
```

The constructor and `Up()`/`Down()` bodies sit under `[assembly: DefaultIntentManaged(Mode.Merge)]` - not
`Body = Mode.Ignore` like a skill's `MarkdownFile` constructor - so a hand-added constructor parameter and
body survive later regenerations. `ModuleId`/`ModuleVersion` stay `[IntentFully]` generated; never hand-edit
them.

## Editing The Consumer's Own Metadata

To read or change the consuming application's own model, add an `Intent.Persistence.V2.IPersistenceLoader`
constructor parameter (package `Intent.Persistence.SDK` - the Software Factory adds the reference
automatically the first time you reference the type, but pins an old/alpha version that can trigger an
`NU1605` downgrade error against `Intent.Modules.Common`'s transitive floor; bump the `.csproj` reference to
a current stable version by hand if that happens - see the `known-build-gotchas` instructions):

```csharp
private readonly IPersistenceLoader _persistenceLoader;

public Migration_01_00_01_Pre_03(IPersistenceLoader persistenceLoader)
{
    _persistenceLoader = persistenceLoader;
}

public void Up()
{
    var application = _persistenceLoader.LoadCurrentApplication();
    var designer = application.GetDesigners().SingleOrDefault(d => d.Id == "<designer-guid>");
    var package = designer?.GetPackages(false, false).FirstOrDefault();
    // package.Classes                       - every element in the package (despite the name, not just "Class")
    // package.FindChildElements(predicate)  - search any element in the package by predicate
    // element.SpecializationTypeId / .Name / .ParentFolderId / .ChildElements
    // package.Classes.Remove(element)       - then package.Save() if anything changed
}
```

This is a **direct, headless edit of the consumer's serialized metadata** - not a live designer session -
so identify designer/element/type IDs by their raw GUIDs/names (from `get_designer_schema`, or a script's
`el.specializationId`), not the `run_designer_script` macro API, which only operates on an open, in-memory
model.

## A Worked Case: Cleaning Up A Consumer's Stale Elements After A Module Restructure

When a module's own File Template layout changes (e.g. flattening a folder, renaming an anchor's sub-path),
the module's own Codebase Structure output moves - but a consumer that installed the older version still
has the old Folder elements sitting in its own Codebase Structure designer model, now orphaned. A
`Version Migration` fixes this directly:

1. **Identify the folder which used to house the template** - locate it by name and known parent (e.g. an

anchor element's `ParentFolderId`), not by assuming anything about its current contents.

2. **Remove its nested folders** - recursively, since a stale folder can itself contain further stale

subfolders (a skill's `resources/` folder, or a deeper `resources/patterns/`).

3. **Remove the folder itself**, then `package.Save()` if anything was removed.
- *Don't gate the removal on "is this folder currently empty."** That was tried and is fragile: whether a

Version Migration runs before or after the Software Factory has relocated a template's output isn't
something the migration can rely on, so a "skip if it still has children" guard can skip forever. Since a
Version Migration only ever runs for a consumer transitioning off the exact old shape it targets, it is
safe to identify the specific obsolete element directly and remove it - and everything nested under it -
unconditionally.

## Checklist

- [ ] Module's version incremented **before** creating the Version Migration element, so the Software

Factory captures the right version.

- [ ] `Module Migrations` container exists (created once, named exactly `Module Migrations`); the new

element is a `Version Migration` child named **exactly** the target semver string.

- [ ] Software Factory run to scaffold the class - the class shell is never hand-written.
- [ ] Constructor parameter (e.g. `IPersistenceLoader`) and `Up()`/`Down()` bodies hand-authored;

`ModuleId`/`ModuleVersion` left untouched (`[IntentFully]`).

- [ ] If a new NuGet package reference was auto-added (e.g. `Intent.Persistence.SDK`), its version checked

against transitive floors - bumped by hand if a `NU1605` downgrade would occur.

- [ ] Cleanup/edit logic identifies its target directly by name/anchor, not by an "already

empty"/"already settled" assumption.

- [ ] Verified against a real consumer application still on the old version - not just a clean build.
