---
description: Guidelines for writing and maintaining C# code in Intent Architect managed .NET applications.
appliesTo:
  - "**/*.cs"
---

## C# using directives

Do not add `using` directives for namespaces already covered by .NET implicit usings or existing `global using` files.

Before adding a `using`, first check:
- the project has `<ImplicitUsings>enable</ImplicitUsings>`
- `obj/**/**/*.GlobalUsings.g.cs`
- any `GlobalUsings.cs` files

Only add explicit `using` directives when the code will not compile without them.