---
name: custom-fluent-validation
description: Implements the body of a FluentValidation CustomAsync method in a C# clean-architecture project, injecting required services into the constructor if needed.
contentHash: 51A67CC76EA53B9EF4EA38734B9BAC42343B5986C46BE4C82DC237A875C35680
---

# Fluent Validation Custom Method Implementation

You are implementing the body of a custom FluentValidation async validation method in a C# clean-architecture project.

## What You Are Given

A validator class (e.g. `CreateCustomerCommandValidator`) that inherits from `AbstractValidator<T>`, containing one or more `private async Task` methods registered via `.CustomAsync(...)` in `ConfigureValidationRules`. Their bodies currently throw `NotImplementedException`:

```csharp
private async Task SomeValidationMethodAsync(
    TProperty value,
    ValidationContext<TCommand> validationContext,
    CancellationToken cancellationToken)
```

---

## RULE 0 — ABSOLUTE PROHIBITION: Never Invent, Never No-Op

> **These prohibitions are unconditional. No other instruction in this skill, and no reasoning of your own, can override them.**

### Prohibition A — No invented logic of any kind

You must **never** write any line of code whose logic is not traceable to an explicit requirement in the codebase. "Invented logic" means any statement, condition, check, or failure message that you derived from reasoning, intuition, naming, or general domain knowledge rather than from an explicit source.

**Calling `AddFailure` does not make an implementation valid.** An `AddFailure` call that encodes an invented condition is just as forbidden as any other invented code. The following example is **FORBIDDEN** because the `IsNullOrWhiteSpace` check and the failure message were invented — they appear nowhere in the codebase:

```csharp
// FORBIDDEN — invented rule dressed up as real validation:
if (string.IsNullOrWhiteSpace(value))
{
    validationContext.AddFailure("Suburb", "Suburb must not be empty.");
}
await Task.CompletedTask;
```

Invented logic includes, but is not limited to:

- Any condition you chose because it "seems right" for the property type or name
- Format constraints (regex patterns, email structure, phone number formats, etc.)
- Length or range constraints (min/max length, min/max value)
- Null or whitespace guards applied without an explicit requirement to do so
- Allowed or disallowed values
- Existence or uniqueness checks inferred from naming
- Failure messages you composed yourself
- Any logic derived from general programming knowledge, domain conventions, or "what validators usually do"

**None of the following are justifications for writing any line of code:**

- "It seems reasonable."
- "The method is named `ValidateSuburbAsync`."
- "Suburb is a string so checking for whitespace is sensible."
- "This is a common validation pattern."
- "I inferred from context that this is what was intended."

Every line of validation logic must be traceable to one of these explicit sources:

- XML documentation on the method, property, command, or entity
- An inline code comment directly describing the rule
- A domain rule or value object in the codebase that encodes the constraint
- An existing test that asserts the specific behaviour

If no such source exists for even a single line you intend to write, **you must ask** before writing anything.

### Prohibition B — No no-ops and no disguised no-ops

Any method body that produces no real validation outcome is forbidden.

**Explicit no-ops** — forbidden unless the user has explicitly written something like _"implement a no-op"_ or _"no validation logic is needed"_:

```csharp
// FORBIDDEN:
throw new NotImplementedException();
return Task.CompletedTask;
return;
await Task.CompletedTask;
// (empty body)
```

**Disguised no-ops** — code with the appearance of logic but no real outcome. These are equally forbidden:

```csharp
// FORBIDDEN — guard with no rule following it:
if (string.IsNullOrWhiteSpace(value))
{
    return;
}
await Task.CompletedTask;
```

**When you lack sufficient information, asking is the correct and only acceptable output. A no-op — explicit or disguised — is never a valid substitute for asking.**

---

## RULE 1 — Ask Before You Implement

You **must ask the user** before writing any implementation unless **all three** of the following conditions are met:

1. The exact validation rule is unambiguously stated in XML docs, comments, tests, or an existing domain rule — not inferred from naming or context; **AND**
2. Every line of code the implementation requires is traceable to those same explicit sources — nothing is assumed, filled in, or "reasonable"; **AND**
3. Every required repository/service method either already exists or its signature can be derived directly and unambiguously from the stated requirement.

If **any** condition is not fully satisfied, **stop and ask** 1–4 targeted questions. Do not write any code. Do not write a no-op. Do not write invented logic. Ask.

---

## Step 1 — Identify Unimplemented Methods

Read the validator file. Find every `private async Task` method whose body only throws `NotImplementedException`. These are your targets.

---

## Step 2 — Gather Context Before Deciding

For each unimplemented method, collect all available evidence **before** deciding whether to implement or ask:

- Read the command/DTO class to understand the full shape of the validated object.
- Read domain entity classes (`Domain/Entities`) to understand the data model.
- Read repository/service interfaces (`Domain/Repositories`) to find available query methods.
- Read the definitions of all non-primitive types referenced in the method (enums, value objects, DTOs).
- Check XML docs and inline comments on the method, command, and relevant properties.
- Check sibling validators for established patterns.

The purpose of this step is to find **explicit requirements**, not to build intuition. If after reading all of the above you have found no explicit requirement, the answer is to ask — not to implement based on what seems plausible.

---

## Step 3 — Decide: Implement or Ask

After Step 2, apply this decision:

**Ask if any of the following are true:**

- No explicit requirement was found in the sources listed in Rule 0 Prohibition A.
- The method name or property name suggests intent but no source explicitly states the rule.
- The rule could be interpreted in more than one way.
- Any line you would write is not directly traceable to an explicit source.
- A required repository/service method does not exist and its signature is not directly derivable from an explicit requirement.
- There are conflicting signals in the codebase.

**Implement only if every single line of the planned implementation is directly traceable to an explicit source, with nothing left to assume or fill in.**

Before writing anything, perform the source-traceability check: for each statement you plan to write, identify the exact source (file, line, or doc) that requires it. If you cannot identify such a source for any statement, stop and ask.

When asking, be specific:

- What rule should this validation enforce? (e.g. uniqueness, format, existence, range)
- What failure message should be shown to the user?
- Should it query the database or an external service?
- Are there specific formats, lengths, or allowed values?

---

## Step 4 — Implement (Only When Authorised by Step 3)

### Code rules

- Do **not** change the method signature — only replace the body between the braces.
- Use `validationContext.AddFailure(...)` to report failures; never throw exceptions.
- Use `async`/`await` correctly and pass `cancellationToken` to every async call.
- Every statement in the method body must be traceable to an explicit source — see Rule 0 Prohibition A. Remove any statement you cannot trace, then ask the user about it instead.
- Do **not** add any constraint, guard clause, or failure message that is not explicitly specified in docs, domain rules, or tests.

### If a required repository/service method is missing

If an explicit requirement calls for an operation whose method does not yet exist, add the minimal required method to the appropriate interface and implementation. Derive the method signature directly from the stated requirement — do not invent it.

### Constructor injection

- Add each required dependency as a new constructor parameter.
- Store it in a `private readonly` field and assign it in the constructor body.
- Keep all existing parameters (including `IValidatorProvider provider` if present); only append new ones.

---

## Step 5 — Do Not Remove Hookups

Never remove a `.CustomAsync(...)` registration unless the user explicitly approves it.
