---
description: Guidelines for writing and maintaining C# code in Intent Architect managed .NET applications.
appliesTo:
  - "**/*.cs"
contentHash: 023F23759EFDDE8A8C1786211D914F595B6053C0371300D1AC16CC0043919D28
---

## C# using directives

Do not add `using` directives for namespaces already covered by .NET implicit usings or existing `global using` files.

Before adding a `using`, first check:
- the project has `<ImplicitUsings>enable</ImplicitUsings>`
- `obj/**/**/*.GlobalUsings.g.cs`
- any `GlobalUsings.cs` files

Only add explicit `using` directives when the code will not compile without them.