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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.RootPrinciples.PrinciplesMd
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PrinciplesMdTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.RootPrinciples.PrinciplesMd";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PrinciplesMdTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("module-building-skills", relativeLocation: "")
                .FromMarkdown(""""""
# Intent Module Building — Reusable Principles

Engineering conventions for writing correct, maintainable Intent Architect module code (templates,
factory extensions, registrations) in C#. Bundled by the `Intent.ModuleBuilder.AI.Skills` module —
sourced from the accumulated conventions used across the Intent Architect module ecosystem.

## Naming Conventions & Standards

### FactoryExtensions & Templates
* **Suffix:** Use `*FactoryExtension` (e.g., `DomainConstraintsFactoryExtension`). One concern per extension; do not merge unrelated cross-cutting concerns.
* **Template Files:**
    * `*TemplatePartial.cs`: Contains constructor, model wiring, and metadata attachment.
    * `*TemplateBase.cs`: Generated; do not hand-edit except for scaffolded `AfterBuild` callbacks.
* **ID Handling:** Prefer using Template Role names (using string constants) over the template's `TemplateId` constant (static `const string`) for lookups. As last resort using hardcoded `TemplateId` strings.

## Architectural Rules

### Engineering Integrity
* **Scan Before You Name:** Search for existing patterns before creating new classes. `grep_search` → `semantic_search` → then decide. Prefer extending abstractions over parallel ones.
* **Access Modifiers:** Define all new types as `internal` by default. Only use `public` if explicitly required for the external API.
* **Shared Projects:** **Do not introduce `.shproj` / `.projitems`** for new components without explicit approval. Prefer a referenced `.csproj` with `PrivateAssets="All"`.

### Implementation Quality
* **Eliminate Magic Values:** Use `const` or `static readonly` fields. No inline magic numbers or strings.
* **Modern Strings:** Use **verbatim literals** (`@"..."`) for quotes and **raw string literals** for multi-line blocks.
* **Builder API First:** When generating or modifying `CSharpFile` code, use the most specific builder API available. Treat raw `AddStatement`, rendered-text replacement, and `GetText()` rewrites as fallback techniques requiring an explicit reason.
* **Warning:** Never use global singletons for template-family scope (state must be clearable between Software Factory runs).

### Template Metadata & Priority Bands
* **Protocol:** Owning templates attach managers in constructors. External extensions use `TryGetMetadata`. Owning templates call `manager.ApplyRules()` in `AfterBuild` at priority `0`.
* **Execution Priorities:**
    | Band | Integer | Usage |
    | :--- | :--- | :--- |
    | **Core** | `0` | Owning template builds primary structure |
    | **Enrichment** | `100` | Same-module cross-cutting additions |
    | **Extension** | `500` | Factory extensions from other modules |
    | **Final** | `1000` | `FindMethod`/`FindClass` on fully-built output |

## Lifecycle Contract

| Phase | Allowed Actions |
| :--- | :--- |
| `OnBeforeTemplateExecution` | Publish events (Registration Requests). **No CSharpFile mutation.** |
| `OnAfterTemplateRegistrations` | Find instances, schedule callbacks, register into managers. **No event publishing.** |
| `OnBuild` / `AfterBuild` | Mutate `CSharpFile`, read metadata, call `ApplyRules`. |

## Build Validation (Mandatory)

After **every** code change, verify the exit code is `0`:
```powershell
dotnet build "path/to/affected.csproj" --no-incremental --verbosity minimal --nologo
```

## C# Using Directives

Do not add `using` directives for namespaces already covered by .NET implicit usings or existing `global using` files.

Before adding a `using`, first check:
- the project has `<ImplicitUsings>enable</ImplicitUsings>`
- `obj/**/**/*.GlobalUsings.g.cs`
- any `GlobalUsings.cs` files

Only add explicit `using` directives when the code will not compile without them.

## Exception Guidelines

When throwing exceptions from module code (templates, factory extensions, registration classes),
choose the right type based on **who the audience is**:

### End-User Exceptions

These are surfaced as formatted messages inside the Intent Architect UI. Use them when the
problem is something the *developer using the module* can fix by changing a designer model or
module setting.

#### `Intent.Exceptions.FriendlyException`

Use when the error is not tied to a specific element — e.g. a missing module, an invalid
combination of module settings, or a misconfigured application.

Write plain prose — **Markdown is not rendered** in FriendlyException messages. Avoid `**bold**`,
backticks, and `\n\n` paragraph breaks; they appear as literal characters in the UI.

```csharp
using Intent.Exceptions;

throw new FriendlyException(
    "OutboxPattern is set to SqlPersistence but Intent.EntityFrameworkCore is not installed. " +
    "The transactional outbox shares the EF Core DbConnection/DbTransaction to guarantee " +
    "exactly-once dispatch. Install Intent.EntityFrameworkCore or change OutboxPattern to None.");
```

Constructor: `FriendlyException(string message)`

#### `Intent.Exceptions.ElementException`

Use when the error is tied to a *specific element* in the designer (a command, event, entity,
stereotype, etc.). Intent Architect uses the element reference to highlight the offending node
in the UI and include its name/location in the error panel.

Write plain prose — **Markdown is not rendered** in ElementException messages. Avoid backticks,
`**bold**`, and `\n\n` paragraph breaks; they appear as literal characters in the UI.

```csharp
using Intent.Exceptions;

// model is e.g. IntegrationCommandModel, EntityModel, etc.
// model.InternalElement implements ICanBeReferencedType
throw new ElementException(model.InternalElement,
    $"Integration Command '{model.Name}' is missing an Endpoint Name. " +
    "Apply the NServiceBus stereotype and set Endpoint Name to the destination endpoint.");
```

Constructors:
- `ElementException(ICanBeReferencedType element, string message)`
- `ElementException(ICanBeReferencedType element, string message, Exception innerException)`

Most model types (e.g. `IntegrationCommandModel`, `MessageModel`, `ClassModel`) expose
`.InternalElement` which is the `ICanBeReferencedType` to pass here.

### Developer Exceptions

Use a plain `InvalidOperationException` (or `NotSupportedException`) for problems that
indicate a **bug in the module itself** — e.g. a switch statement missing a new enum value,
an unexpected null that should never occur if the module is wired correctly. These are not
shown in a friendly panel; they appear as raw stack traces in the SF output.

```csharp
// Developer exception — module has a bug
throw new InvalidOperationException($"Unsupported transport type: {transport.Value}");
```

### Decision Table

| Situation | Exception type |
|---|---|
| Missing module install | `FriendlyException` |
| Invalid module setting combination | `FriendlyException` |
| Element missing required stereotype/property | `ElementException` |
| Element has an invalid configuration | `ElementException` |
| Unhandled enum value in a switch (module bug) | `InvalidOperationException` |
| Unexpected null that should never be null (module bug) | `InvalidOperationException` |
| Generated runtime code (e.g. `?? throw`) | Leave as `InvalidOperationException` — runs at app startup, not SF time |

### Notes

- Both `FriendlyException` and `ElementException` live in the `Intent.SoftwareFactory.SDK`
  NuGet package (namespace `Intent.Exceptions`). No extra package reference is needed in module
  projects — the SDK is already a transitive dependency.
- Generated code strings (e.g. `method.AddStatement(@"... ?? throw new InvalidOperationException(...)")`)
  are emitted into the user's application and run at startup, not during SF execution. These
  are always `InvalidOperationException` regardless of audience — the user sees them in their
  own app logs, not in the IA UI.
- Markdown is supported in both `FriendlyException` and `ElementException` messages. Prefer
  short, actionable messages: state what is wrong, then state the fix.
"""""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}