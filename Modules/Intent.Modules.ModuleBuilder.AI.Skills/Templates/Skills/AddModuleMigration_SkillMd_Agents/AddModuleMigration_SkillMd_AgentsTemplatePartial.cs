using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.AddModuleMigration_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AddModuleMigration_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.AddModuleMigration_SkillMd_Agents";

        internal const string SkillName = "add-module-migration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AddModuleMigration_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
                .FromMarkdown($$""""""
                    ---
                    name: {{SkillName}}
                    description: "Add a Version Migration (or On-Install/On-Uninstall Migration) that programmatically edits an already-installed consumer application's own persisted metadata during install/update. USE ONLY WHEN a module's own restructuring, rename, or model/stereotype shape change needs applications that installed an earlier version to converge onto the shape a fresh install already gets. DO NOT USE FOR generating code into a consumer via the module's own templates (see file-builder-expert) or for bumping the module's own version number (see module-version-increment). REQUIRES the target module's Module Builder designer already open, and its version already incremented for the change being migrated."
                    template-id: {{TemplateId}}
                    keywords: [migration, version-migration, on-install, on-uninstall, persistence, consumer-metadata, module-update]
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

                    **The `Version Migration` element's name must be the exact semver string it targets** — e.g. `1.0.1-pre.3`,
                    not a descriptive name like `FlattenSkillsFolders`. Confirmed against a real shipped module's Migrations
                    tree (`Intent.Modules.VisualStudio.Projects`): `On-Install`, `3.8.10-pre.2`, `3.9.2-pre.0`, `4.0.0-pre.0`,
                    … one entry per version that ever needed a migration. The generated class name derives directly from the
                    element name (dots/hyphens become underscores, numbers are zero-padded — `1.0.1-pre.3` →
                    `Migration_01_00_01_Pre_03`), and its `ModuleVersion` property is the element name verbatim. Both come
                    from the element name alone, not from the module's version; `ModuleId` is the module's name. The
                    properties are `[IntentFully]`, so never hand-edit them — rename the element instead.

                    **Increment the module's version first, then create the Version Migration element, named for that new
                    version.** A migration runs for a consumer updating across the version in its name, so the name must be the
                    unpublished version that ships the change. Name it after a version consumers already have and they never
                    run it. Bumping first means you know the right name before you create the element.

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

                    The other two elements generate the same way, each with a single method to fill in. The class name is the
                    element's name plus `Migration` (an element named `OnInstall` gives `OnInstallMigration`), and there is no
                    `ModuleVersion`:

                    ```csharp
                    public class OnInstallMigration : IModuleOnInstallMigration
                    {
                        public OnInstallMigration()
                        {
                        }

                        [IntentFully]
                        public string ModuleId => "...";

                        public void OnInstall() { }
                    }

                    public class OnUninstallMigration : IModuleOnUninstallMigration
                    {
                        public OnUninstallMigration()
                        {
                        }

                        [IntentFully]
                        public string ModuleId => "...";

                        public void OnUninstall() { }
                    }
                    ```

                    ## Editing The Consumer's Own Metadata

                    To read or change the consuming application's own model, add an `Intent.Persistence.V2.IPersistenceLoader`
                    constructor parameter, from the `Intent.Persistence.SDK` package. Check the package reference before you
                    write any code against it:

                    - The `Version Migration` template always adds `Intent.Persistence.SDK` to the module's `.csproj`, pinned at
                    `1.0.1-alpha.2`. That version has only the older `Intent.Persistence` namespace; the `Intent.Persistence.V2`
                    API shown below needs `1.0.3`. Raise the `.csproj` reference to `1.0.3` by hand. If the module references
                    `Intent.Modules.Common` 3.11.0 or later (which depends on `1.0.3`), leaving it at `1.0.1-alpha.2` is also an
                    `NU1605` downgrade error - see the `known-build-gotchas` instructions.
                    - The `On-Install Migration` and `On-Uninstall Migration` templates add no package. If the module has no
                    Version Migration, add the `Intent.Persistence.SDK` `1.0.3` reference yourself.

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
                    so identify designer/element/type IDs by their raw GUIDs/names (look them up beforehand with
                    `run_designer_query` - `getDesignerSchema()`, or an element handle's `el.getId()` / `el.specializationId`),
                    not the `run_designer_script` macro API, which only operates on an open, in-memory model.

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

                    **Don't gate the removal on "is this folder currently empty."** That was tried and is fragile: whether a
                    Version Migration runs before or after the Software Factory has relocated a template's output isn't
                    something the migration can rely on, so a "skip if it still has children" guard can skip forever. Since a
                    Version Migration only ever runs for a consumer transitioning off the exact old shape it targets, it is
                    safe to identify the specific obsolete element directly and remove it - and everything nested under it -
                    unconditionally.

                    ## Checklist

                    - [ ] Module's version incremented **before** creating the Version Migration element, so the element can be
                    named for the new, unpublished version (the name is what becomes `ModuleVersion`).
                    - [ ] `Module Migrations` container exists (created once, named exactly `Module Migrations`); the new
                    element is a `Version Migration` child named **exactly** the target semver string.
                    - [ ] Software Factory run to scaffold the class - the class shell is never hand-written.
                    - [ ] Constructor parameter (e.g. `IPersistenceLoader`) and `Up()`/`Down()` bodies hand-authored;
                    `ModuleId`/`ModuleVersion` left untouched (`[IntentFully]`).
                    - [ ] `Intent.Persistence.SDK` reference raised to `1.0.3` in the `.csproj` (the Version Migration template
                    adds `1.0.1-alpha.2`, which lacks `Intent.Persistence.V2`), or added by hand for an On-Install/On-Uninstall
                    migration.
                    - [ ] Cleanup/edit logic identifies its target directly by name/anchor, not by an "already
                    empty"/"already settled" assumption.
                    - [ ] Verified against a real consumer application still on the old version - not just a clean build.
                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
