---
contentHash: 29DA188658061327DDF216C805EDE85B1848AF9616AF90302FBE6C1BD953A0CA
---
# Integration Recipe — Wiring A Module Against A Modelers Designer

The full three-file wiring, with every snippet lifted verbatim from `Intent.Modules.Metadata.RDBMS`
— the cleanest consumer in this repo, where all three files agree at the same version (`3.11.1`)
against the `Domain` designer. Substitute `Domain` for whichever designer you're integrating with
(`Services`, `Eventing`, `User Interface`) and use that designer's own resource file for its real
current version and identity — do not copy the `3.11.1` number itself, it is illustrative only.

## 1. `.csproj` — compile-time

```xml
<PackageReference Include="Intent.Modules.Modelers.Domain" Version="3.11.1" />
```

The NuGet `PackageId` always carries the `Intent.Modules.` prefix — this is what your project
compiles against, and what gives you `using Intent.Modelers.Domain.Api;`.

## 2. `.imodspec` — install-time

```xml
<dependency id="Intent.Modelers.Domain" version="3.11.1" />
```

- *Note the different id string** — no `Modules.` segment. This is the identity the Intent Architect

installer resolves when your module is installed into a consuming application; get it wrong and the
install fails, not the compile, so the mistake surfaces much later than the `.csproj` one would.

## 3. `modules.config` — design-time visibility in your own Module Builder designer

```xml
<module moduleId="Intent.Modelers.Domain" version="3.11.1" includeAssets="none" supportedClientVersions="[4.3.0-a, 5.0.0-a)" />
```

- *You do not write this entry — you install it.** `modules.config` records what is actually installed

in the application; hand-editing it corrupts that record. Install the designer module into your
module-building application instead, and in the Installation Settings dialog tick **`Install Designer
Metadata` and nothing else**:

| Setting | Module-building application |
|---|---|
| **Install Designer Metadata** | tick — this is the one you want |
| **Install Designers** | rare; only if the element types still do not appear with metadata alone |
| Enable Factory Extensions | leave clear |
| Install Application Settings | leave clear |
| Install Template Outputs | leave clear |

That choice is what produces `includeAssets="none"` above — the attribute is the *result* of a
metadata-only install, not something you type. If it is absent, the install was done with the wrong
boxes ticked; redo the install rather than editing the file.

What this buys you: the designer's element/type vocabulary becomes available to *your own* Module
Builder package while you are modelling — e.g. so a `Template Settings` stereotype's `Model Type`
property can resolve `ClassModel` — without dragging that module's factory extensions, settings or
generated output into an application whose only job is to define a module.

## 4. The `using` + accessor call

```csharp
using Intent.Modelers.Domain.Api;

var classModels = metadataManager.Domain(application).GetClassModels();
```

The API namespace (`Intent.Modelers.Domain.Api`) is one level deeper than the module id
(`Intent.Modelers.Domain`) — it always adds a trailing `.Api` segment. Never assume the namespace
equals the module id with `.Api` appended to the *NuGet* package id; it's appended to the module id.

## 5. Publishing your own stereotypes onto a foreign designer

If your module needs to attach its own stereotypes onto elements owned by a designer you don't own
(e.g. RDBMS attaching `Table`/`Column`/`Index` stereotypes onto `Domain`'s `ClassModel`), model those
stereotype definitions in your own Module Builder package, then install them into both the target
designer and your own Module Builder designer so the stereotype editor works at design-time too:

```xml
<install target="Domain;Module Builder" src="Metadata/Domain/Intent.Metadata.RDBMS.pkg.config" externalReference="AF8F3810-745C-42A2-93C8-798860DC45B1" />
```

The `target` attribute is a semicolon-separated list of designer names — `Domain;Module Builder` is
the shape to copy, substituting the designer(s) you're actually extending.

## The `Api/` naming convention — how to answer "what's in this designer" for one this skill doesn't cover

Every fact in `domain.md` / `services.md` / `eventing.md` / `user-interface.md` was derived
mechanically from a designer module's generated `Api/` folder. Use this table to answer the same
questions yourself for a fifth designer, or to re-verify a fact that may have drifted since these
resource files were written:

| Looking for | Read |
|---|---|
| Element types | `Api/<X>Model.cs` → `SpecializationType` + `SpecializationTypeId` consts |
| Association types | `Api/<X>AssociationModel.cs` (⚠️ Eventing spells it `Assocation`) |
| Association navigation | `Api/Extensions/<X>ModelAssociationExtensions.cs` — **Domain & Services**; `Api/<X>ModelAssociationExtensions.cs` directly — **Eventing & User Interface** |
| Stereotypes | `Api/<X>ModelStereotypeExtensions.cs` |
| Mappings | `Api/ElementToElementMappingExtensions.cs` (often in an extension module, not the base designer — e.g. Services' mapping lives in `.DomainInteractions`, not the base `Services` module) |
| Package model | `Api/<X>PackageModel.cs` |
| Cross-element abstractions | `Api/I<X>Model.cs` |
| Accessor + designer GUID | `ApiMetadataDesignerExtensions.cs` at the **module root**, not under `Api/` |
| Publish/subscribe-filtered accessors (Eventing only) | `MetadataManagerExtensions.cs` at the module root, namespace `Intent.Modules.Modelers.Eventing` — **not** `Api/ApiMetadataProviderExtensions.cs`, and **not** the `.Api`-suffixed namespace |

Stereotype ownership is not uniform across designers, and this is the single most likely thing to
get wrong: `Domain` and `Services` define **no stereotypes of their own** — every stereotype you'll
see on a `ClassModel` or `DTOModel` (`Table`, `Column`, `HttpSettings`, `Secured`, …) arrives from a
separate `Intent.Metadata.*` module that must be installed and referenced independently, with its
own full install identity. `User Interface` is the opposite — it ships
`ComponentModelStereotypeExtensions`, `PropertyModelStereotypeExtensions` and
`EventEmitterModelStereotypeExtensions` directly in its own `Api/`. Always check which case a
designer is in before assuming a stereotype either belongs to it or comes from elsewhere.
