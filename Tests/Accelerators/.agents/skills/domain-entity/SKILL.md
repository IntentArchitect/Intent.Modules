---
name: domain-entity
description: guide coding agents to implement missing c# domain behaviour on a single domain entity or aggregate that lives in a dependency-free clean architecture domain project and may be persisted by a technology like ef core. use when a user shares a c# domain class with missing methods, not implemented exceptions, incomplete constructors, weak invariants, or unclear placement of business logic, and they want help finishing the domain behaviour while keeping persistence concerns secondary.
contentHash: B1A85F64978296250C25582FF97CD981F2925B5E28FF63325BF82A986BEFCDFB
---

# Domain Entity

Implement or improve business behaviour on a single C# domain class.

Treat the class as part of a clean architecture domain model first and an persistence type second. Preserve domain intent, protect invariants, and avoid pushing business rules into application services unless the rule genuinely spans multiple aggregates or external systems.

## Default workflow

1. Read the entity or aggregate carefully.
2. Identify missing behaviour, especially methods throwing `NotImplementedException`, empty methods, placeholder guards, or constructors that do not fully establish a valid object.
3. Infer the domain intent from names, fields, properties, comments, existing guards, and related methods.
4. Implement the smallest coherent set of domain rules needed to make the type useful and internally consistent.
5. Explain assumptions briefly when the domain intent is uncertain.

## What to optimise for

- Keep business rules on the entity, aggregate root, or value object when they protect that type's own invariants.
- Prefer explicit domain operations over exposing setters.
- Make invalid states hard to create.
- Keep code usable in a domain-only project with no infrastructure dependencies.
- Align with the style already present in the user's codebase unless it is clearly harmful.

## Constructor guidance

When constructors are incomplete or missing, use these rules:

- Ensure a public constructor or factory establishes a valid domain object.
- Validate mandatory arguments.
- Normalise values only when the model already suggests it or when it is clearly required.

## Operation guidance

When implementing methods on the class:

- Enforce invariants at the point of state change.
- Check whether the method should be idempotent.
- Update all related fields together so the object cannot drift into a partially valid state.
- Prefer meaningful domain exceptions only if the codebase already uses them. Otherwise use standard argument or invalid operation exceptions.
- Return values only when the domain operation naturally produces one.
- Avoid side effects that belong to infrastructure.

## How to infer missing behaviour

When a method body is missing, infer intent from:

- method name and parameters
- property names and existing state
- nearby guard clauses
- comments, XML docs, or tests if provided
- common domain patterns already used in the class

If the intent is still ambiguous, choose the safest low-surprise implementation and state the assumption clearly.

## Implementation rules

- Preserve existing naming and coding style where reasonable.
- Do not introduce unnecessary abstractions.
- Do not add MediatR, repositories, domain services, or events unless the user already uses them or asks for them.
- Do not rewrite the entire model into a more opinionated DDD style unless the user requests that.
- Do not convert behaviour into extension methods.
- Do not expose mutable internals just to make the implementation easier.

## Before finishing, quickly verify that you have:

- Kept the business behaviour on the entity, aggregate root, or value object where it belongs.
- Implemented the missing methods, guards, or constructors that were clearly incomplete.
- Preserved or strengthened invariants so invalid state is harder to create.
- Validated mandatory inputs and rejected impossible state transitions.
- Updated related fields together so the object stays internally consistent.
- Avoided introducing infrastructure concerns into the domain model.
- Avoided adding unnecessary abstractions, patterns, or dependencies.
- Preserved the existing naming and coding style unless it was clearly harmful.
- Chosen standard exceptions unless the codebase already uses domain-specific ones.
- Stated any important assumptions briefly where domain intent was ambiguous.
- Returned only the code and explanation needed to complete the domain behaviour cleanly.