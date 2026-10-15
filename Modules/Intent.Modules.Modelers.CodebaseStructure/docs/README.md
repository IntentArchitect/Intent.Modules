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

### The *File Classification* stereotype

Flags files for Change Review with a Review Importance and one or more classifications (the `File Classification` elements under the root's `File Classifications` element). Its rules are rows in the `Entries` property, each with a `Glob`, a `Review Importance` and `Classification`s. Where several rows match a file, the last matching row wins.

It can be applied to:

| Element | Covers | Globs are relative to |
|---|---|---|
| **Root Folder** (`root`) | Custom files anywhere in the application, e.g. `NuGet.Config` or `.editorconfig` at the top level | The application's output location |
| **Folder**, and elements that opt in such as **C# Project** and **Solution Folder** from [Intent.VisualStudio.Projects](https://docs.intentarchitect.com/articles/modules-dotnet/intent-visualstudio-projects/intent-visualstudio-projects.html) | Custom files under that folder or project | The folder's or project's directory (each contained project's root for a solution folder) |
| **Template Output** | Only the files that template generates | The folder containing the `Template Output` |

For a custom file, the deepest element with a matching row wins, so a folder or project's own rows override the root's, and the root's rows apply wherever nothing deeper matches. A generated file is classified only by its `Template Output`.

## Related Modules

### [Intent.VisualStudio.Projects](https://docs.intentarchitect.com/articles/modules-dotnet/intent-visualstudio-projects/intent-visualstudio-projects.html)

Adds Visual Studio Solution and C# Project modeling on top of this designer's `Root Folder`, `Folder`, and `Output Anchor` elements, and is the primary consumer of the `Root Folder Options` stereotype's `Relative Location` shift (`.sln`/`.slnx`, `.gitignore`, `Directory.Packages.props`).
