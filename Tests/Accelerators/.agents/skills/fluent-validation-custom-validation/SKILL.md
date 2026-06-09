---
name: fluent-validation-custom-validation
description: implement or revise fluent validation custom async method logic in an existing validator file. use when a c# fluent validation validator has an incomplete or incorrect custom async validation method and the agent should update the method body, add private helper methods, and extend application or domain abstractions such as repositories or services if required, while avoiding direct infrastructure dependencies in the validator.
template-id: Intent.Application.FluentValidation.CustomValidationSkillTemplate
contentHash: 40B5DE04A23E86965C2B06AAABE3833178B39E6AE8FB452F4271AA99C393609E
---
# Fluent Validation Custom Async Method

Implement custom async validation method logic inside an existing validator file. Keep validators aligned with the modeled domain and existing validation patterns while protecting architectural boundaries.

## Core rules

- Treat the existing validator file as the starting point. Update the method body rather than rewriting unrelated parts.
- NEVER modify the method signature of the custom validation method.
- ALWAYS if you modify a custom validation method, ensure that the `IntentManaged` attribute indicates that the body of the method must be in `Mode.Ignore` (e.g. `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`).
- Inject only dependencies from the Domain or Application layers.
- Never introduce direct dependencies on infrastructure packages or infrastructure types in the validator, including Entity Framework, Dapper, concrete DbContexts, SQL clients, or vendor-specific data access abstractions.
- If data access is required, favor using an existing repository abstraction from the Domain or Application layers, but extend them if required.
- Every line of validation logic must be traceable to an explicit requirement in the codebase: XML documentation, inline comments, domain rules, value objects, or existing tests. Never invent logic based on naming, intuition, or general domain knowledge.
- Search the codebase for similar validators, domain rules, validation patterns, and repository methods before introducing a new approach.
- Add private helper methods inside the validator when they improve readability, rule composition clarity, or encapsulate repeated validation logic.
- Keep the validator focused on orchestrating validation checks and delegating business rules to domain/application abstractions when those patterns already exist nearby.
- Use `validationContext.AddFailure(...)` to report validation failures; never throw exceptions from validation methods.

## Workflow

1. Inspect the existing validator, command/DTO, custom validation method, repository interfaces, and related domain types, such as enums.
2. Search for code usages of:
  - similar validators and their custom async methods
  - domain rules, value objects, and policies that encode constraints
  - repository or service methods supporting similar validation checks
  - existing test cases that assert expected validation behavior
3. Gather explicit evidence from sources: XML docs, comments, domain entities, tests, and value objects.
4. Only implement if every single line of code is traceable to an explicit requirement; otherwise, ask targeted questions.
5. Implement the method body using existing patterns first.
6. If the validator needs missing DAL capabilities, extend the relevant repository abstraction in an allowed layer instead of pulling infrastructure into the validator.
7. Add focused private helper methods when validation logic becomes easier to read or test.
8. Ensure cancellation tokens are threaded through async calls where applicable.
9. Verify the final code preserves layer boundaries and avoids infrastructure leakage.

## Validation-specific guidance

Custom async validation methods usually check business rules that require external data access. Prefer patterns like these when supported by the surrounding codebase:

- retrieve required data through an existing repository or domain service
- apply business-rule checks consistently with nearby validators and domain entities
- reuse established domain constraints, specifications, or value objects
- report failures using domain-appropriate language aligned with existing messages
- avoid computing derived facts in memory; delegate to domain/application abstractions when possible

## Repository and abstraction extension guidance

When a needed validation capability is missing:

- Extend the existing repository or domain service closest to the business concept.
- Keep method signatures expressive in domain terms rather than storage terms.
- Prefer names such as `ExistsByAsync`, `IsAvailableAsync`, `CheckBusinessRuleAsync`, or `ValidateAsync` over schema-oriented names.
- Do not explain or encode infrastructure implementation details in the validator.
- Do not reference how the repository will be implemented in EF, Dapper, SQL, or similar.
- Prefer adding "using" statements for the namespace of newly introduced aspects instead of referencing them with fully qualified names in the validator (unless there is a namespace conflict)

## Requirement gathering

Before implementing any custom validation method, ensure you have found explicit evidence. If no explicit requirement is found in these sources, ask the user:

- XML documentation on the method, property, command, or entity
- Inline code comments directly describing the validation rule
- Domain rules, value objects, or policies that encode the constraint
- Existing unit tests that assert the expected behavior

If you find no such evidence, ask:

- What validation rule should this method enforce? (e.g. existence, uniqueness, format, range, business invariant)
- What failure message should be shown to users?
- Should it query the database, call a domain service, or check a value object?
- Are there related tests, domain rules, or examples in the codebase?

## Output expectations

Produce a concrete code update that:

- fills in or corrects the custom async method body
- adds private helper methods in the validator if useful
- extends allowed-layer abstractions when needed for the validator to be correct
- preserves the existing feature style and naming
- keeps unrelated refactors out of scope

You must **never** write any line of code whose logic is not traceable to an explicit requirement in the codebase. "Invented logic" means any statement, condition, check, or failure message that you derived from reasoning, intuition, naming, or general domain knowledge rather than from an explicit source
Explicit no-ops are forbidden unless the user has explicitly written something like _"implement a no-op"_ or _"no validation logic is needed"
Disguised no-ops - code with the appearance of logic but no real outcome. These are equally forbidden
When you lack sufficient information, asking is the correct and only acceptable output. A no-op — explicit or disguised — is never a valid substitute for asking

## Review checklist

Before finishing, check that:

- every injected dependency belongs to the Domain or Application layers
- the validator imports no infrastructure package namespaces
- repository changes, if any, are expressed as abstractions only
- every line of validation logic is traceable to an explicit requirement in the codebase
- the implementation follows existing code usage patterns rather than inventing a novel style without reason
- business language matches the surrounding domain model
- the code path is cancellation-aware and async-safe where relevant
- no invented validation rules, constraints, or failure messages appear in the code
