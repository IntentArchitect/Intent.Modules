# Intent.Modelers.CodebaseStructure

This module provides the [Codebase Structure Designer](https://docs.intentarchitect.com/application-development/modelling/codebase-structure-designer/codebase-structure-designer.html), used to model where an application's generated code and supporting files are physically placed on disk — its `Root Folder`, `Folder`s, `Output Anchor`s, and `Template Output`s. Other modules build on top of these elements (for example, [Intent.VisualStudio.Projects](https://docs.intentarchitect.com/articles/modules-dotnet/intent-visualstudio-projects/intent-visualstudio-projects.html) adds Visual Studio Solution and C# Project modeling) and read this designer's metadata to resolve, for every generated file, which physical folder it belongs in.

## Stereotype details

### The *Root Folder Options* stereotype

This stereotype is applied to the **Root Folder** element (shown as `root` at the top of the Codebase Structure Designer).

#### Relative Location

Shifts the application's absolute output root (`OutputRootDirectory`) uniformly for every consumer that resolves a location relative to it, while an output relying on its default (Name-based) location generates back at its original location.

This is intended for scenarios where the application's own `Relative Output Location` setting has been pointed at a specific project's own leaf folder — for example, so that tooling resolving a project's location (such as Reveal in Code Base Explorer) can pinpoint it directly — while shared, root-level output still needs to be generated one or more levels up, in a folder shared with sibling applications.

Known consumers of this shift today all come from [Intent.VisualStudio.Projects](https://docs.intentarchitect.com/articles/modules-dotnet/intent-visualstudio-projects/intent-visualstudio-projects.html):

- The `.sln`/`.slnx` file.
- `.gitignore`.
- A centrally-managed `Directory.Packages.props`.

For example, if `OutputRootDirectory` has been set to `Container\MyProject` and you want the `.sln` generated at `Container\MyProject.sln` (alongside `MyProject`, not inside it), set `Relative Location` to `..`.

Leave `Relative Location` blank for no shift — this is the default and matches all prior behavior.

> [!NOTE]
> An output with its own explicit relative location — for example, a Visual Studio project placed inside a materialized Solution Folder (see [Intent.VisualStudio.Projects](https://docs.intentarchitect.com/articles/modules-dotnet/intent-visualstudio-projects/intent-visualstudio-projects.html)) — is not affected by this shift; only an output relying on its default location participates.

> [!WARNING]
> If `Intent.Modules.SharedKernel.Consumer` is also installed, its solution-patching feature scans for the `.sln` file directly under `OutputRootDirectory` and will not find it once `Relative Location` is non-blank, silently skipping that step.

## Related Modules

### [Intent.VisualStudio.Projects](https://docs.intentarchitect.com/articles/modules-dotnet/intent-visualstudio-projects/intent-visualstudio-projects.html)

Adds Visual Studio Solution and C# Project modeling on top of this designer's `Root Folder`, `Folder`, and `Output Anchor` elements, and is the primary consumer of the `Root Folder Options` stereotype's `Relative Location` shift (`.sln`/`.slnx`, `.gitignore`, `Directory.Packages.props`).
