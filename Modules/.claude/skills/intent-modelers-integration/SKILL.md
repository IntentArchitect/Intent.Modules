---
name: intent-modelers-integration
description: "Look up install identity, entry points, and element/association/stereotype references for Intent's built-in Domain, Services, Eventing, or User Interface designer before wiring a module against one. USE ONLY WHEN a module needs to reference, install, or generate against one of Intent's own Domain/Services/Eventing/User Interface designers. DO NOT USE FOR reading stereotypes off a model you already hold (see intent-metadata-consumer) or for authoring the C# builder statements themselves (see file-builder-expert). REQUIRES knowing which designer(s) the module needs to integrate with."
argument-hint: "[designer name] [module purpose]"
template-id: Intent.ModuleBuilder.AI.Modelers.Skills.IntentModelersIntegration_SkillMd_Agents
contentHash: 6DEC29FD38889414ABE06AB121F6F0C8078CF1440877E1FD9DD0BD171EDE42BE
---
# Intent Modelers Integration

> [!TIP]
> **Read more if you want to know about** the exact three-file wiring recipe, or a specific designer's elements/associations/stereotypes/extension modules in full:
> *   [Integration Recipe](./resources/integration-recipe.md) | [Domain](./resources/domain.md) | [Services](./resources/services.md) | [Eventing](./resources/eventing.md) | [User Interface](./resources/user-interface.md)
> *(To conserve tokens, only read the resource file(s) for the designer(s) your module actually touches.)*

## Musts

1. **Four identities, never interchangeable.** For any designer module `X`: NuGet PackageId = `Intent.Modules.Modelers.X` (`.csproj` `PackageReference`); Intent module id = `Intent.Modelers.X` — no `Modules.` — (`.imodspec` `<dependency>`, `modules.config`); API namespace = `Intent.Modelers.X.Api` (`using`); assembly = `Intent.Modules.Modelers.X.dll`. Rule of thumb: NuGet package = `Intent.Modules.` + module id minus its leading `Intent.`. The split is a deliberate `RootNamespace`/`PackageId` mismatch baked into the designer's own `.csproj`, not an accident — see `resources/integration-recipe.md` for the worked, verbatim example.
2. **The dependency must land in three places at the same version, but you only author two of them.** Hand-write the `.csproj` `PackageReference` (compile-time) and the `.imodspec` `<dependency>` (install-time). **`modules.config` is never hand-edited** — it records what is actually installed, and a bad edit corrupts the application's module state. Its entry comes from *installing* the designer module into the module-building application, metadata-only (Must #7).
3. **Reach elements through the generated extension chain** — `_metadataManager.<Designer>(app).Get<X>Models()`, never raw `GetElementsOfType` with a hand-typed GUID.
4. **Bind template registrations to the typed model** — `FilePerModelTemplateRegistration<ClassModel>` (or the designer's own equivalent model type).
5. **To attach your own stereotypes to a foreign designer**, model them in your own Module Builder package and ship `<install target="Domain;Module Builder" …>` (substituting the target designer name(s)).
6. **Check every extension module's identity independently** — never infer one extension module's NuGet id / module id / namespace pattern from a sibling. The four designers' extension modules are inconsistent with each other on this (see each designer's resource file § "Extension modules"); the only safe move is reading that module's own `.csproj`/`.imodspec`, every time.
7. **Install a designer module metadata-only.** In the Installation Settings dialog tick **`Install Designer Metadata`** and leave the rest clear. A module-building application references a designer so its *element types become visible to model against* — it is not running that designer's generation. Ticking `Enable Factory Extensions`, `Install Application Settings` or `Install Template Outputs` pulls the referenced module's factory extensions, settings and generated output into an application whose only job is to define a module — which is how a module-builder starts emitting code nobody asked for. `Install Designers` is the one rare addition: use it only when the element types you need still do not appear with metadata alone.

## Must Nots

1. Never write `Intent.Modules.Modelers.Domain` as an `.imodspec` dependency id, or `Intent.Modelers.Domain` as a `PackageReference` — both fail, and confusingly late.
2. Never add a `PackageReference` to `Intent.Modules.<X>.Api` — **no such package exists.** (A `.Api` suffix on a *module id* like `Intent.Entities.Repositories.Api` is a different thing entirely: a role/contract module.)
3. Never let the three versions drift — real modules in this repo do (e.g. `Services.DomainInteractions`' `.csproj` reference trails its own `.imodspec` dependency floor); don't copy a stale number from a resource file without checking the current one.
4. Never hand-edit a file under `Api/` carrying `[assembly: IntentTemplate(...)]` — it is generated.
5. Never assume an extension module's namespace mirrors its parent designer, or that its module id keeps (or drops) the `Modules.` segment the way another extension module did — each one is independently inconsistent (e.g. `Domain.StoredProcedures` keeps `Modules.` in both its module id and namespace where its four siblings drop it).
6. Never hand-edit `modules.config` to add, change or "fix" a dependency — including typing `includeAssets="none"` yourself. That attribute is the *result* of a metadata-only install, not an input; if it is missing, the install was done with the wrong boxes ticked, so redo the install rather than patching the file.
