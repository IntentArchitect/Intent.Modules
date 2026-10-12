---
description: The governing Intent Architect doctrine — the model is the source of truth, what must be modelled, how to protect bespoke code from regeneration, and when a task is actually done. Read before changing any code in an Intent Architect solution.
alwaysApply: true
intent-updates: automatic
---

## The model is the source of truth

Intent Architect models are the source of truth; the codebase is a generated artifact. If a user
instruction conflicts with these rules:

- Default to the safer Intent Architect approach.
- Do not bypass modelling for contracts or structure.
- Only proceed with direct code changes if the user explicitly confirms after being made aware of
  the modelling alternative.

Never infer structure, naming, or hierarchy from files on disk, and never hand-edit generated code
to change structure — change the model and re-generate. Always completely trust Intent Architect's
tool results over what the codebase appears to say.

## What must always be modelled

The following must **never** be changed directly in code:

- Method signatures
- API contracts
- DTO shapes
- Service interfaces
- Routing and endpoint definitions
- Persistence structure (schema, entity mappings, relationships)

**Heuristic**: if you can describe a change in terms of operations, contracts, entities, services,
repositories, events, or architectural relationships, model it.

## Allowed exceptions (bespoke code)

Direct code editing is allowed **only** for:

- Method bodies inside handlers and services.
- Dependency injection inside bespoke implementation constructors. Do not modify generated DI
  registration if it can be modelled.
- Bodies of repository methods and custom queries (the interface, entity mappings, and persistence
  schema remain model-owned).
- Business rules that genuinely cannot be expressed in models — a rare last resort. Ask the user
  before invoking this exception.

Avoid partial classes unless they are already part of existing project patterns.

### Protecting bespoke code from regeneration

Custom logic must be protected with one of:

- `[IntentIgnoreBody]`
- `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`

```csharp
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    [IntentIgnoreBody]
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(request.CustomerId, request.Items);
        await _orderRepository.AddAsync(order, cancellationToken);
        return order.Id;
    }
}
```

The attribute sits on the **member**, not the class. The signature stays generated; only the body is
preserved. Anything hand-written outside such a slot is reclaimed on the next Software Factory run.

## Detect generated code before editing

Before editing a file, check for generated markers — `[IntentManaged(...)]` attributes (C#) or the
equivalent in other stacks, and auto-generated file headers. If markers are present, **do not edit
directly**: model the change instead. For a new file, check whether its sibling files are generated.

## Off-limits files

Never read or modify anything owned by the designers and the Software Factory:

- Anything inside `.intent` or `Intent.Metadata` folders.
- Any `.application.config` file.

## Identifiers

Never invent IDs — only use IDs returned by Intent Architect tools. Do not put IDs in plans or
messages shown to the user; reference elements by name and type.

## The `intention` parameter

Intent Architect tools that declare an `intention` parameter require it — your intent in 10 words or
fewer. This applies to **Intent Architect tools only**. Host-native tools (`Read`, `Write`, `Edit`,
`Grep`, `Glob`, `Bash`, `PowerShell`, …) have their own schemas and will reject `intention` as an
unexpected parameter.

## Documentation lookup

Use `search_docs` for questions about Intent Architect features, designers, attributes, code
management, or workflow concepts before answering. Not for trivial or unrelated questions.

## Definition of done

The task is complete only when the capability is **modelled** with no validation errors, the
Software Factory has been run and applied, the codebase compiles and tests pass, and any required
bespoke logic is in place and survives a clean re-run — no `NotImplementedException` / `TODO` /
stubs in new files.

- If the request is **read-only** (explain, summarize, compare, audit), just inspect — do not
  mutate, generate, or edit code.
- If the Software Factory fails, or applied changes don't compile, **report the error to the user**.
  Do NOT patch generated code to make it compile: it masks the modelling error and is overwritten on
  the next run.
