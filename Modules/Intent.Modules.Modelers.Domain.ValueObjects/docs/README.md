# Intent.Modelers.Domain.ValueObjects

A **Value Object** is a domain concept defined entirely by its values rather than by an identity — for example an `Address` or a `Money` amount. Two Value Objects with the same values are interchangeable.

The `Intent.Modelers.Domain.ValueObjects` module adds the `Value Object` element to the **Domain Designer**. How a Value Object is realized in code depends on the other modules installed, for example *Intent.ValueObjects* and your persistence module.

## Modeling a Value Object

Right-click a Domain package, folder or diagram and select **`New Value Object`** (`ctrl + shift + v`), then add its values with **`Add Attribute`** (`ctrl + shift + a`).

A Value Object can be used as the type of:

- an Attribute
- an Operation's return type
- a Parameter, including Service Operation parameters and return types

## Associations

An entity can be associated to a Value Object, and a Value Object can be associated to another Value Object.

To start an association from a Value Object, right-click it and select **`New Association`** (`ctrl + shift + c`), then choose `One to One`, `One to Zero-or-One` or `One to Many`. Selecting `New Association` itself creates a `One to One` association.

Only composite relationships are offered. When the target of an association is a Value Object, the source end is always set to exactly one (not a collection and not nullable): the Value Object is owned by the element that points to it.

A Value Object can't be associated to an entity. Value Objects are immutable values and shouldn't refer back to the entities that own them, and the code-generation modules don't support it. Such an association is shown as a validation error on the Value Object:

> Value Object 'Address' cannot be associated to Entity 'Country'. Value Objects can only be associated to other Value Objects.
