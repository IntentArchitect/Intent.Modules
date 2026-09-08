# CONTEXT.md — Intent.Modelers.Services.EventInteractions

## AI Context additions (v2.0.8)

- `Subscribe Integration Event`/`Subscribe Integration Command`: a chained domain interaction (Create/Query/Update/Delete Entity Action) must attach from the association's own Target End node, not the `Integration Event Handler` element — the handler itself has no such context-menu option, only its Target End does.
- `Publish Integration Event`/`Send Integration Command` own `Publish Message Mapping`. Same policy as [[Intent.Modelers.Services.DomainInteractions/CONTEXT.md]]: don't map unless the user explicitly asks — wire only the association, `// IntentIgnore` the publish call, hand-write field assignment in code. Avoid `mapExpression` for strings when mapping is used.
- `Subscribe Integration Event`/`Subscribe Integration Command`: added a "Diagram layout" bullet — place the message/handler node and this association's Target End node together in ONE `apply_change_diagram_layout` call and route the edge there too. Cause: an agent wiring a new message + subscription in one pass split placement/routing across calls and errored/wheelspun, even though the general rule already exists in `intent-architect-mcp` SKILL.md ("an edge routes only when both endpoint nodes are placed in the same call") — the generic rule wasn't surfacing at the point of decision for this specific cross-designer pattern, so it's repeated here where the attach-point rule already lives.

Keep additions here terse — bullet the symptom/rule, no elaboration.
