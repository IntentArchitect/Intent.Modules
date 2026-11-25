---
description: Guidelines for writing and maintaining C# code in Intent Architect managed .NET applications.
appliesTo:
  - "**/*.cs"
paths:
  - "**/*.cs"
applyTo: '**/*.cs'
contentHash: 1AD87A97FFD4B7B2543099B10A64A6DEB4633715CC337C6DF92EE475E5689AA6
---

## C# using directives

Do not add `using` directives for namespaces already covered by .NET implicit usings or existing `global using` files.

Before adding a `using`, first check:
- the project has `<ImplicitUsings>enable</ImplicitUsings>`
- `obj/**/**/*.GlobalUsings.g.cs`
- any `GlobalUsings.cs` files

Only add explicit `using` directives when the code will not compile without them.