# Intent.Metadata.DocumentDB

This module provides `Domain Designer` metadata and scripting support for Document DB orientated modules.

## Domain Designer modeling

The `Domain Designer` has been extended with several stereotypes for modeling Document Db technology specific concepts in your domain.

### Document Database - Package stereotype

The `Document Database` stereotype is applied to a `Domain Package`, and configures it to use the Document DB modelling paradigm.

The stereotype may be applied automatically but can also be applied manually to `Domain Packages` if required.

This stereotype has a Provider property, to specify which specific type of Document Db technology the Domain Package should be realized in. This drop down has the following options.

- Default (None selected), if no provider is specified and you have a single Document DB Provider module installed (e.g. Intent.CosmosDB), that module will be used by default.
- Custom, the backing implementation will need to be implemented through custom code.
- Dynamic installed module providers (e.g. CosmosDB, MongoDd, Dapr), any Document DB Provider implementing modules will show as options here.

If you have multiple Document DB technologies you would need to configure which Domain Packages are for which Document DB technologies.

![Document Database](./docs/images/document-database-stereotype.png)

### Primary Key - Attribute stereotype

The `Primary Key` stereotype indicates that an `Attribute` is the document's primary key.

By default any `Class`'s added to your domain will have an `Attribute` added named `Id` with the `Primary Key` stereotype applied to it.

This stereotype is visualized as a golden key icon.

![Primary Key visual](./docs/images/primary-key-stereotype.png)

#### Primary Key Type

The default type for the `Primary Key` is `Object ID` (string). This can be changed using the `Document Database > Id Type` application setting.

The available options are:

- **Object ID (string)** - the default
- **GUID**

![Primary Key Type](./docs/images/primary-key-types.png)

#### Primary Key Creation

The default behavior in terms of `Primary Key` creation, is for the `Primary Key` to be created on **all** entities (with the exception of child entities with one-to-one relationships)

This behavior change be changed using the `Document Database > Key Creation Mode` application setting.

The available options are:

- **All** - the default. All modeled entities, except for *child entities with one-to-one relationships*, will automatically be assigned a `Primary Key`.
- **Only on Documents** - Only parent entities (no *child entities*) will automatically be assigned a `Primary Key`.

A visual example of an `All` modeled *one-to-many* relationship - the child entity (OrderItem) **has** a `Primary Key`:

![Child Primary Key](./docs/images/all-primary-key.png)

A visual example of an `Only on Documents` modeled *one-to-many* relationship - the child entity (OrderItem) **does not** have a `Primary Key`:

![Child Primary Key](./docs/images/only-documents-primary-key.png)

> [!NOTE]
> The `Create CRUD CQRS Operations` accelerator in the `Service Designer` requires that an entity has a `Primary Key` (that the `Key Creation Mode` is set to `All`). If a child entity does not have a `Primary Key`, then entity will not be available for selection when using the `Create CRUD CQRS Operations` accelerator.

### Foreign Key - Attribute stereotype

The `Foreign Key` stereotype indicates an `Attribute` has been introduced to a `Class` as a result of a modeled `Association`, for example:

![Foreign Key visual](./docs/images/foreign-key-stereotype.png)

In this diagram you can see the `CustomerId` attribute has been introduced, with the `Foreign Key` stereotype, as a result of the many-to-one relationship between `Basket` and `Customer`.

For the Document DB paradigm, association between different aggregate roots are denotes by dotted line associations as these are references between documents.

The `Foreign Key` stereotype's are automatically managed when modeling associations. This stereotype is visualized as a silver key icon.

#### Compositional Relationship

This is the default relationship when modeling an `Association` and is represented by a **black diamond** on the source end of the relationship, and a **solid line** between the two entities.

With this relationship, the child entity is consisted part of the parent entity, and cannot exist without a parent.

In this example, an *OrderItem* has a `Compositional Relationship` with *Order*, and cannot exist independently of an *Order*. It is considered part of the *Order*.

![Compositional Relationship](./docs/images/compositional-relationship.png)

#### Aggregational Relationship

When the `Source End` of the `Association` has been set as `Is Nullable`, the relationship is considered an `Aggregational Relationship`. This is represented by a **white diamond** on the source end of the relationship, and a **dotted line** between the two entities.

This relationship is modeled by setting the `Is Nullable` property of the `Source End` of the `Association` to **true**. With this relationship, the "child" entity, can exist independently of the "parent" entity.

In this example, an *Address* has a `Aggregational Relationship` with *Order*, and can exist independently of an *Order*.

![Aggregational Relationship](./docs/images/aggregational-relationship.png)
