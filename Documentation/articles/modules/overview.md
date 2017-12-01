# Modules Overview
Modules are the building blocks and extension points of Intent Architect. They typically encapsulate _how_ your metadata should be realized as code in your application. While the outputs of each Module are not restricted to a particular language, creating the Modules must be done using C# and .NET (a modern and powerful strongly typed language).

## Creating Modules
If you're looking to automate patterns unique to your project, then creating your own Modules is the way to do it. It's easy to get started by following [this document](../getting_started/downloadable_example_module.md) and _checking out_ the example code. For more in depth examples, _check out_ our [GitHub repository of modules](https://github.com/IntentSoftware/IntentArchitect), which has many working modules that can be used as is in your applications, or as a seed for your own custom patterns.

See [this document](../getting_started/creating_a_module_from_scratch.md) for a full tutorial on creating a module from scratch.


## Module Components
Every Module requires an **[imodspec file](imodspec_file.md)** to describe it and what components it contains.
- **[Templates](../templates/overview.md)** - Templates are the artifacts which are responsible for generating code.
- **[Decorators](decorators.md)** - Decorators effectively extend the output of templates that support them. This is very useful when writing templates that are going to be used across projects and need to be extensible and flexible.
- **[Factory Extensions](factory_extensions.md)** - Extend the software factory by hooking into the execution life-cycle, template life-cycle, or output transformations. Can be used to alter output from templates, change metadata, run installers, or anything really.
- **[Metadata](imodspec_file.md)** - Install predefined metadata into your application on installing of the Module. This is very useful when you have predefined Stereotypes or Types that your templates depend upon.

## Packaging
To package a Module from a `csproj` (C# Project) into a Module that is able to be installed into an Intent Architect application, a packager is required. This is provided as a NuGet package `Intent.IntentArchitectPackager`, which can be installed by running the following:

```
Install-Package Intent.IntentArchitectPackager
```

On successful compiling of the project, the Module will be packaged into a folder at `%SolutionFolder%\Intent.Modules`. By creating a repository at this folder from within Intent Architect's Modules manager, you can install this module into your application.