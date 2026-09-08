# CONTEXT.md — Intent.Modelers.Services.EventInteractions

## AI Context additions (v2.0.8)

- `Subscribe Integration Event`/`Subscribe Integration Command`: a chained domain interaction (Create/Query/Update/Delete Entity Action) must attach from the association's own Target End node, not the `Integration Event Handler` element — the handler itself has no such context-menu option, only its Target End does.
- `Publish Integration Event`/`Send Integration Command` own `Publish Message Mapping`. Same policy as [[Intent.Modelers.Services.DomainInteractions/CONTEXT.md]]: don't map unless the user explicitly asks — wire only the association, `// IntentIgnore` the publish call, hand-write field assignment in code. Avoid `mapExpression` for strings when mapping is used.

Keep additions here terse — bullet the symptom/rule, no elaboration.
