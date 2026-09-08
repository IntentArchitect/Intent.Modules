# CONTEXT.md — Intent.Modelers.Services.DomainInteractions

## AI Context on `Create Entity Action` / `Update Entity Action` (v2.4.8)

Observed AI-modeling mistakes on Create/Update Entity advanced mappings:

- Fields get Data-Mapped but the `Invocation Mapping` (constructor call) is left unwired, so the target stays required-but-unmapped.
- `mapExpression` used for string interpolation, producing invalid C#.

`AI Settings.Rules` on both associations now calls these out directly. Keep additions here terse — bullet the symptom and the fix, no elaboration.

Default policy: wire only the minimum needed to satisfy designer validation (the `Invocation Mapping` mapped end), then `// IntentIgnore` the generated method body and hand-write the field logic in code — not "leave it fully unmapped" (that fails validation). Full field mapping via the designer only when the user explicitly asks for it.
