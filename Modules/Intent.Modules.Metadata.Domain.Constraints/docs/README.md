# Intent.Metadata.Domain.Constraints

This Intent Architect module adds Domain Designer metadata for modeling validation rules on entity attributes.

## What It Provides

Use the `Add Domain Constraint` context menu on a domain attribute to apply common constraints:

- `Required`
- `Text Limits` (Min Length / Max Length)
- `Numeric Limits` (Min Value / Max Value)
- `Collection Limits` (Min Length / Max Length)
- `Regular Expression` (Pattern / Message)
- `Email`
- `Url`
- `Base64`

These constraints are metadata-driven and can be consumed by downstream modules to generate technology-specific validation implementations.

## FluentValidation Integration

When this module is used together with modules such as `Intent.Application.FluentValidation.Dtos` and `Intent.Application.MediatR.FluentValidation`, the modeled domain constraints can be realized as generated FluentValidation rules in your application layer.

In other words, you model the constraints once in the Domain Designer, and compatible FluentValidation modules can generate corresponding validation rule definitions from that metadata.

## How To Apply

1. Open the Domain Designer and select an `Attribute` on an entity.
2. Right-click and choose `Add Domain Constraint`.
3. Select the desired constraint.
4. Configure the constraint properties where applicable.

![Add Domain Constraints](images/add-domain-constraints.png)