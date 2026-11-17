# Intent.Modelers.Domain

The `Intent.Modelers.Domain` module provides the **Domain Designer**, a UML class diagram style designer for modeling entities, their attributes and operations, and the relationships between them.

Please see [the docs](https://docs.intentarchitect.com/articles/application-development/modelling/domain-designer/modeling-the-domain/modeling-the-domain.html) to learn about modeling the Domain.

## Associations and Generalizations on diagrams

Entity `Association` and `Generalization` lines are only drawn on Domain designer diagrams. Other designers that show Domain classes on their diagrams, such as the Services designer, don't draw them, so showing a Domain class's related elements there doesn't bring them in.

If a diagram in another designer already holds one of these lines, it's reported as a visual that failed to load. Select the error to remove it from the diagram.
