### Version 1.1.6

- Improvement: Updated Document db Domain Modeler scripts to support Table Storage paradigm.

### Version 1.1.5

- Fixed: Foreign Keys that aren't linked with an Association (i.e. orphaned) are now getting removed.

### Version 1.1.3

- Improvement: Support for multiple document DB providers added.

### Version 1.1.2

- Improvement: Primary Keys are now updated on generalization changes.

### Version 1.1.1

- Improvement: Primary keys are now also managed for composite entities. If you require not having a primary key on class then use a `Value Object`.

### Version 1.1.0

- Improvement: Cleaned up the internal scripts which now show where to find their TS script equivalents in order to make modifications.
- Fixed: Fixed a bug where the Domain Modeler association change script not managing keys correctly.

### Version 1.0.1

- Improvement: dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.0.0