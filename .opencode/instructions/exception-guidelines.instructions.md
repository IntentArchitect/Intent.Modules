---
contentHash: 49ACAB46CCA86B16260336FF94B8E1F8A659B07560F77AEFC416B93CE58A18F2
---
## Intent Module Exception Guidelines

When throwing exceptions from module code (templates, factory extensions, registration classes),
choose the right type based on **who the audience is**, and on **where the code runs**:

===

### End-User Exceptions

These are surfaced in the Intent Architect Software Factory output as readable errors rather than raw
stack traces. Use them when the problem is something the *developer using the module* can fix by
changing a designer model or module setting.

#### `Intent.Exceptions.FriendlyException`

Use when the error is not tied to a specific element — e.g. a missing module, an invalid
combination of module settings, or a misconfigured application.

Throw it only **outside build callbacks**: from a template registration, a factory extension, or a
template constructor. Inside an `OnBuild` / `AfterBuild` callback the file builder catches every exception
that is not an `ElementException` and replaces it with a generic "An unexpected error occurred while
building [file]" error, demoting your message to an inner exception; the same happens to anything thrown
while a template renders its output. Inside a callback, throw `ElementException` instead.

Intent Architect renders the message as Markdown, but write it so it also reads cleanly as plain text:
command-line runs and logs print the raw message. Inline code and bold are fine; avoid headings,
tables, lists and links that only make sense once rendered.

`FriendlyException` was added in `Intent.SoftwareFactory.SDK` 3.9.0. A module scaffolded with Module
Builder's defaults references an older SDK (3.7.0), so raise the `Intent.SoftwareFactory.SDK` package
reference in the module's `.csproj` to 3.9.0 or later before using it. The Software Factory then checks
the `.imodspec`'s `supportedClientVersions` against that SDK version; 3.9.0 needs Intent Architect 4.5.0
or later.

```csharp
using Intent.Exceptions;

throw new FriendlyException(
    "OutboxPattern is set to SqlPersistence but Intent.EntityFrameworkCore is not installed. " +
    "The transactional outbox shares the EF Core DbConnection/DbTransaction to guarantee " +
    "exactly-once dispatch. Install Intent.EntityFrameworkCore or change OutboxPattern to None.");
```

Constructor: `FriendlyException(string message)`

===

#### `Intent.Exceptions.ElementException`

Use when the error is tied to a *specific element* in the designer (a command, event, entity,
stereotype, etc.), and for any end-user error raised inside a build callback. Intent Architect uses
the element reference to let the user navigate to the offending element and to show its name in the
error.

Write plain prose — **Markdown is not rendered** in ElementException messages. Avoid backticks,
bold, and blank-line paragraph breaks; they appear as literal characters in the UI.

```csharp
using Intent.Exceptions;

// model is a generated designer model (e.g. ClassModel);
// model.InternalElement is the IElement to pass
throw new ElementException(model.InternalElement,
    $"Integration Command '{model.Name}' is missing an Endpoint Name. " +
    "Apply the NServiceBus stereotype and set Endpoint Name to the destination endpoint.");
```

Constructors:

- `ElementException(ICanBeReferencedType element, string message)`
- `ElementException(ICanBeReferencedType element, string message, Exception innerException)`

What you can point at: `ICanBeReferencedType` is implemented by elements (`IElement`), packages
(`IPackage`) and association ends (`IAssociationEnd`). Generated models expose them as
`InternalElement` (element models), `UnderlyingPackage` (package models) and `InternalAssociationEnd`
(association end models), so a package-level or association-level problem can point straight at the
package or the association end.

Properties, all set from the element when the exception is created: `ElementId`,
`ElementDisplayText` (the element's display text followed by its specialization type in brackets),
`ApplicationId` and `DesignerId` (from the element's package). They are what Intent Architect uses to
navigate to the element, and what a test can assert on to check which element a failure points at.

`ElementException` is available at every SDK version Module Builder scaffolds; it needs no package
change.

===

### Developer Exceptions

Use a plain `InvalidOperationException` (or `NotSupportedException`) for problems that
indicate a **bug in the module itself** — e.g. a switch statement missing a new enum value,
an unexpected null that should never occur if the module is wired correctly. These are not
shown in a friendly panel; they appear as raw stack traces in the SF output.

```csharp
// Developer exception — module has a bug
throw new InvalidOperationException($"Unsupported transport type: {transport.Value}");
```

===

### Decision Table

| Situation | Exception type |
|---|---|
| Missing module install | `FriendlyException` |
| Invalid module setting combination | `FriendlyException` |
| Any end-user error detected inside `OnBuild` / `AfterBuild` | `ElementException` (on the template's model, or the element at fault) |
| Element missing required stereotype/property | `ElementException` |
| Element has an invalid configuration | `ElementException` |
| Package or association end has an invalid configuration | `ElementException` (pass the `IPackage` / `IAssociationEnd`) |
| Unhandled enum value in a switch (module bug) | `InvalidOperationException` |
| Unexpected null that should never be null (module bug) | `InvalidOperationException` |
| Generated runtime code (e.g. `?? throw`) | Leave as `InvalidOperationException` — runs at app startup, not SF time |

===

### Notes

- Both `FriendlyException` and `ElementException` live in the `Intent.SoftwareFactory.SDK`
  NuGet package (namespace `Intent.Exceptions`). `ElementException` needs no package change;
  `FriendlyException` needs the SDK reference raised to 3.9.0 or later (see above).
- Generated code strings (e.g. `method.AddStatement(@"... ?? throw new InvalidOperationException(...)")`)
  are emitted into the user's application and run at startup, not during SF execution. These
  are always `InvalidOperationException` regardless of audience — the user sees them in their
  own app logs, not in the IA UI.
- Markdown is rendered only for `FriendlyException`, never for `ElementException`. For both, prefer
  short, actionable messages: state what is wrong, then state the fix.
