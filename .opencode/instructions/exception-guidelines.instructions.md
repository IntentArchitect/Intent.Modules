---
contentHash: 282B3DFD82972E746A4B4F8340684671DEB2128ABD87E901895DDBEC7EE105A0
---
## Intent Module Exception Guidelines

When throwing exceptions from module code (templates, factory extensions, registration classes),
choose the right type based on **who the audience is**:

===

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

===

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
| Element missing required stereotype/property | `ElementException` |
| Element has an invalid configuration | `ElementException` |
| Unhandled enum value in a switch (module bug) | `InvalidOperationException` |
| Unexpected null that should never be null (module bug) | `InvalidOperationException` |
| Generated runtime code (e.g. `?? throw`) | Leave as `InvalidOperationException` — runs at app startup, not SF time |

===

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
