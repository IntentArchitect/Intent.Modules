---
name: module-building-strategies
description: "The accumulated strategic playbook of judgment calls for designing an Intent Architect module — decomposition (root/bridging/common), template vs factory-extension choice, file cardinality, managed modes, setting-vs-stereotype, convention-vs-explicit, pattern-first building from a specification, two-phase verification, and how a module carries AI guidance. USE ONLY WHEN facing a design decision point while building or extending a module, or planning large module work from a specification or PRD. DO NOT USE FOR the mechanical how-to of a specific construct — this is the judgment layer above those."
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleBuildingStrategies_SkillMd_Agents
contentHash: C59072C02329C0B1F4B142C9FB407CEB3AAEE6FA385D9ABC977AA7DB9D061E05
---
# Module Building Strategies

This skill captures *the judgment* — the strategic instincts behind building Intent Architect modules, distinct from the mechanical steps of scaffolding one. Load it whenever you face a design decision, not just a mechanical step.

> **Governing philosophy:** Intent does not dictate how things should work. It gives the developer maximum flexibility to accomplish what's needed, **built on top of industry best practices**. Every strategy below serves that philosophy.

===

## Discover Before You Decide — Module Reconnaissance

This skill holds the **WHY** (strategy/judgment). It deliberately does **not** enumerate per-module **WHAT** — which base class the MassTransit consumer uses, what `Intent.Common.CSharp` offers, which request types exist, what version of a package to use. Those facts are **discoverable and volatile**: documenting them here would rot, and the code/platform already hold the current truth.

**Rule: never assume a per-module fact from memory or from a skill. Discover it fresh each build, then interpret it through the strategy below** (code shows *what* a module does, rarely *why*).

Read order — cheapest and most authoritative first:

1. **`search_docs`** — IA features, designers, attributes, code-management, module concepts.
2. **`run_solution_query`** — `searchAvailableModules(id, { includePrerelease: true })` for what's available, `listInstalledModules(appId)` for what's installed.
3. **The reference module's own source** — `imodspec` (dependencies, interoperability, version), template registrations (roles, base classes, file cardinality), `NugetPackages.cs` (packages + conditions), factory extensions (what they enrich, which broadcast requests they publish/handle).
4. **The reference app's actual generated output** — ground truth for what the ecosystem already produces; don't re-generate what's already provided.
5. **`run_designer_query`** — `getDesignerSchema` for element types, stereotypes and settings; `searchElements` and `getElementDetails` for the modelled elements that drive generation.

> This protocol is the durable replacement for static per-type "playbooks": it stays current automatically, costs no developer time, and scales to any module.

### Offload discovery to keep the orchestrator lean

Reconnaissance and research are context-heavy and read-only — ideal to **delegate to a sub-agent** (if your harness supports background/sub-agent dispatch) so the main conversation's context stays lean for the work only it can do (designer / SF / model mutation). Hand it a focused discovery brief and take back the distilled findings; if no sub-agent mechanism is available, do the discovery inline.

### Resolve conflicts at the authoritative source

When two modules **disagree** about how a stereotype (or property) is used — one reads it correctly, another (often a legacy or copied module) reads the wrong property — do **not** arbitrate by copying whichever sibling "looks right." Trace back to the **authoritative source: the stereotype's *definition*** (where it is declared), and let that decide which usage is correct. That trace is also what exposes the real culprit — the discrepancy usually points at a bug in a *different* module than the one you were changing. (This is why blind sibling-mimicry is unreliable: the sibling may be the one that's wrong.)

### Patterns you'll find but must not copy

Reading existing modules is the fastest way to learn the platform, but some of what published modules do is legacy that a new module must not repeat. When reconnaissance turns one of these up, note it and do the current thing instead.

- **AI task providers.** Some published modules (Intent.Modules.NET's Wolverine and MediatR-family modules among them) register an AI task provider from a factory extension — `application.AITaskManager.RegisterTaskProvider(...)` — to flag generated handlers that still need writing. AI task providers are deprecated: never register one. Carry the guidance as described in §5.
- **Interaction strategies.** Intent.Modules.NET's handler modules implement `IInteractionStrategy` and register it with `InteractionStrategyProvider` to turn a modelled interaction (create, query, update or call an entity) into a generated handler body. That approach is deprecated: an AI agent writes business logic better than a deterministic strategy can, so a new module must not add interaction strategies. Generate the handler's structure, emit its body as an `IgnoreBody()` stub (§2 "Managed modes"), and carry the guidance the agent needs as described in §5.

===

## 1. Module Architecture & Decomposition

### Vertical slices — decompose the *work* before the modules

For a large PRD, don't build everything at once. Decompose the work into **vertical slices** — each a *feature* the developer can model and see work end-to-end, which may span **several modules** (e.g. one module for the designer elements, another for code extensibility, another for a different codegen aspect). A slice is the tracking layer **above** the per-module Attack Plan; the units nest:

`PRD → vertical slice (a feature) → spanning set (distinct shapes within it) → increment (one SF cycle)`

Cut a slice so it is **independently valuable and independently testable** — it carries its own acceptance check. Build one slice to green, test it whole, then move to the next; apply the module-decomposition judgment below within each slice.

### Prove a code pattern before you template it

A specification says what a module should generate; it does not prove that code works. For large work — a specification or PRD, several new templates, or a new module — a template only reproduces a code pattern that is already proven: hand-written code in a reference application that compiles, runs and is tested. Small work, a change to one template or a single new template, needs no reference code; when you cannot tell which size the work is, ask the developer.

For large work:

1. **List the code patterns the work needs**, and where each one's reference code is. A pattern with no hand-written code, no tests, a new library or library version, or a description in prose only is unproven.
2. **If a pattern is unproven, stop and ask the developer** how to proceed: they supply reference code, you write a reference implementation for them to review before any template reproduces it, or the scope narrows to the proven patterns. Never work a pattern out inside a template; a template is the worst place to discover how the code should look.
3. **Template one pattern at a time.** Reproduce the reference code in one template, verify it (§4), then widen it to the spanning set.

**Checkpoints.** If the developer supplied a plan, it sets the review points; add none of your own. Without one, ask the developer before templating the first pattern whether you can carry on through the rest yourself or should stop for their review after each pattern.

### The dependency boundary is the module boundary

This is the foundational rule. A module must only carry the code and NuGet dependencies it genuinely needs.

- **Root modules** are stand-alone technologies that work by themselves — ASP.NET Core, EF Core, MediatR. A root module must **never force a specific technology onto another root module** (ASP.NET Core must never drag in MediatR).
- If two modules can interoperate through **generic / shared interfaces** with no new dependency → keep it as is, **no split needed**.
- The moment integrating them would require a module to take on a **dependency it doesn't otherwise need** → extract that dependency into a **bridging module**.

### Bridging modules

A bridging module exists to carry an integration dependency so neither root module has to. It depends on **both** sides (e.g. a MediatR↔Controllers bridge depends on both), keeping each root module dependency-clean. It is **installed on-demand, never by default**, and supports swapping alternatives (a different dispatcher gets its own bridge).

A common bridging mechanism: root technology modules (Controllers, FastEndpoints, Azure Functions) **ship NuGet packages exposing specific interfaces** so the Services layer can supply specially-crafted models to bridge technologies *without* a direct dependency. The bridge consumes those interfaces.
> ⚠ *Open question under review:* whether NuGet-exposed bridging interfaces are the best long-term approach. Treat as the current convention, not settled doctrine.

### Common modules

A **common module** encapsulates a shared role or shared infrastructure — usually scoped to one suite of modules, not universal (e.g. `Intent.Common`, `Intent.Common.CSharp`, `Intent.Common.TypeScript`).

Common modules are the **correct answer to the diamond-dependency / single-version problem**:

> When modules are backed by C# shared projects and two modules each reference the *same third shared project that isn't itself a module*, each module **bundles its own copy** of that shared DLL. The module loader binds to the **first** module's copy. If the second module was built against a different (out-of-sync) version, you get version skew → runtime trouble. A common module fixes this structurally: the shared code lives in one place, versioned centrally → a single authoritative DLL, no duplicate bundling, no skew.

So common modules aren't just tidier — they prevent a real DLL-skew hazard. **Lead with this when justifying a common module.**

### Shared projects are discouraged

`.shproj` / `.projitems` shared projects were a **cost-avoidance workaround**, not a principled choice. Standing up a real module carried overhead — technical *and* user/maintenance overhead — so shared projects let the team share code while avoiding that cost (and, as a side effect, let shared APIs mature before committing to a module contract).

**AI changes the economics.** When AI can create and maintain modules, that overhead largely dissolves and the justification falls away. Therefore:

- **Discourage new shared projects.** Prefer a referenced `.csproj` with `PrivateAssets="All"`, or a common module.
- **Existing shared-project code is a migration candidate.** The HTTP client family (the typical HttpClient module, the Dapper client, Blazor's HttpClient, gRPC) shares substantial infrastructure via shared projects and is the canonical case that should graduate into a **common module** (not yet extracted).

> **Meta-principle:** Strategies that exist only as *pre-AI cost compromises* should be re-evaluated under AI maintenance. The cheap option changes when the overhead changes.

### Broadcast over direct coupling

When an integration style recurs across modules, **encapsulate it abstractly in the Common C# module as a broadcast → handle mechanism** rather than wiring modules together directly. A module *broadcasts* a request; whichever module is responsible *handles* it. Current examples:

- **DI registration** — `ContainerRegistrationRequest`
- **App settings** — `AppSettingRegistrationRequest`
- **Infrastructure resource registration** — broadcast resources other modules consume; especially useful for **health checks**

The heuristic: **don't couple — broadcast.** (Mechanism details for the registration requests live in `intent-module-orchestrator`.)

===

## 2. Template Mechanics

### Template vs factory extension — mechanical, not a judgment call

- A **template's sole purpose is to generate a file.** If the concern produces a new output file → it's a template.
- A **factory extension cannot generate its own files.** It only extends other templates or wires up capabilities. If the concern *modifies another module's output* or wires behavior without emitting a file → it's a factory extension.
- A template *can* be made to also do factory-extension-like work, but this is **rare and reserved for specific nuanced reasons** — never the default.

### File cardinality

| Kind | When | Receives |
|---|---|---|
| **File-per-model** | One output file per modeled element (e.g. one file per entity) | A single model instance |
| **Single-file with model list** | One output file aggregating many elements (e.g. one file referencing all entities) | The collection of models — iterate over them |
| **Single-file, no model** | A **static** generated file, invariant of any model (config class, assembly marker, static helper) | Nothing |

The choice is purely: how many files, and does one file need to see many models at once.

### Managed modes

- **`Mode.Fully`** — Intent fully manages the space; Intent owns it end-to-end.
- **`Mode.Merge`** — Intent and the developer **coexist** in that space. Intent compares each run's output with its previous output, so it knows which lines it wrote and keeps changing and removing those, leaving the developer's own lines intact. So an agent writing code there has to leave Intent's lines alone without being able to see which ones they are.
- **`Body = Mode.Ignore`** (`CSharpIntentManagedAttribute.IgnoreBody()`) — the template keeps the signature; the body is written when the member first appears, as a starting point or a stub, and then belongs to the developer or the AI implementing it.
- **`Mode.Ignore`** on a whole file or member — **opts out of code automation entirely**, signature included. It is **not** how a module hands a body to a developer or AI. Its one common use in module output is a small member scaffolded once for the developer to own outright, such as a factory extension's `Order` property.

**Decision rule:** give every method body one owner.

- **Code the template can generate from the model** → a **fully** managed body.
- **Code a developer or AI writes** → **`Body = Mode.Ignore`**, with a stub (`file-builder-expert` Musts #11). This is the default for a new template's "implement here" body.
- **Both in one body** → pull the generated part into its own fully managed member, such as a private method or a helper class, and call it from the ignored body. A model change still reaches the generated member; if it changes the member's signature, the build breaks rather than the body going quietly stale.
- **Merge** → where the module's `CONTEXT.md` or the developer says generated and hand-written lines share the body (as handler templates in several existing modules do). The one case to choose it yourself is a body that collects generated lines from several templates or modules, such as a DI registration method, where nothing can be pulled out. `// IntentInitialGen` above a once-off statement in a Merge body lets the developer replace or delete it without the Software Factory restoring it.

**Never change an existing template's body mode on your own.** Moving a Merge body to `Ignore` freezes every consumer's current body, and moving the other way starts rewriting code consumers own. It is a decision for the developer, recorded in the module's `CONTEXT.md`.

===

## 3. Design-Time Configuration

> First distinction: **design-time** configuration (drives generation) vs **runtime** configuration (connection strings, names — belongs in appsettings). The rules below are about *design-time* config only.

### Setting vs stereotype vs association

- **Module setting** → **application-wide** behavior/rule. Its scope is the whole app, regardless of how anything is modeled.
- **Stereotype** → **fine-grained, element-level** configuration, often used to **override or specialize a global setting** on specific elements. (Configuration is only one use of stereotypes, not their whole purpose.)
- **Associations have nothing to do with configuration** — they are a structural modeling mechanism. Never reach for an association to capture a setting.

The common shape is **global default (setting) + per-element override (stereotype)**. Canonical example — **multitenancy**: a *module setting* picks the global isolation level (database-level = one DB per tenant, app-wide; vs entity-level = one shared DB); when entity-level, a *stereotype* on each entity marks it multitenant and gives it a discriminator.

> **Choosing a stereotype has a packaging consequence.** A stereotype only reaches consumers if its
> owning package is marked `Include in Module` in that package's `Module Settings`. Tick it as you define
> the stereotype — otherwise the package emits no `<install>` entry, the stereotype ships nowhere, and the
> only signal is a Software Factory warning you have to notice. See `add-designer-extension` for the
> companion `Reference in Designer` rule when the package also carries designer elements.

### Convention vs explicit — the three lenses

When a module needs information to generate correctly, decide in this order:

1. **What does the industry say about this technology?** If there's an established convention, **adhere to it and default to it** — don't force the developer to specify what the convention already determines. (E.g. publishing an event should just work, using the conventional identifier — typically the message's fully-qualified namespace.)
2. **What does the technology itself offer / constrain?** Where the tech is free-form with no clear convention → it's **debatable**; work it through with the client/AI. Where a **technical limitation** prevents relying on the convention (e.g. a transport that needs an explicit topic/queue name it can't infer) → **break out of convention and make it explicit/mandatory** — prompt the developer.
3. **How can Intent make it simple** to leverage all this? Developer ergonomics is the final lens.

So: convention by default when the industry provides one; explicit capture when the technology can't honor it; debate when the tech is free-form.

===

## 4. Verification Strategy

### Two-phase verification — reproduce, then generate from scratch

Verifying a module is two phases, **in sequence** — not either/or:

**Phase 1 — reproduce on the reference app.** The reference app first proves the *architecture* works before you automate it. Then install the module *on top of that same reference architecture* and confirm the module outputs **exactly what the hand-written code already has**. This validates that the templates reproduce the proven output.

**Phase 2 — from-scratch test (mandatory, the gap-exposer).** Once Phase 1 is 100% confirmed, build a **brand-new app**, install the module **with no code pre-written**, and confirm Intent generates everything correctly and fully wired. This is the **only** test that exposes a forgotten wiring step — as a compile error, a runtime error, or "the code just looks wrong." Phase 1 *cannot* catch this because the code was already there.

> This is exactly why "SF shows 0 changes" on the reference app proves nothing on its own — the code was already present. The from-scratch run is the real proof.

### When the module cannot be installed

Both phases need the module installed in an application. If the install fails, or the module cannot be found to install, stop verifying and report it with the error and what you tried — if the module-building workflow instructions are present, their "If A Phase Cannot Be Completed" section is the procedure. Neither phase counts as passed: finish what needs no install (build, documentation, dependency audit by reading the code) and mark the generated output as unverified. Never edit `modules.config` or anything under `.intent` to force the install through.

### Extensions on another modeler's element types

When the module extends element types owned by a different modeler (an Element Extension on another designer's element, say), that modeler must be installed in the module's **own** Module Builder application — a metadata-only install is enough. Without it the extension has no type to point at, and the Software Factory cannot write it into the module's designer settings. Check this before Phase 1. Then, in each verification app, confirm the modeler is installed there too and that the extension actually appears on the target elements; an extension that is missing from the designer was not verified, however clean the generation looked.

### The regeneration-baseline gate (Phase 1 precondition)

Phase 1's comparison is only valid if SF genuinely regenerates the template-owned regions. Before trusting any staged diff:

1. Identify every file the module's templates will own.
2. **100%-Intent-managed files** → **delete-to-regenerate** so SF produces them fresh and the diff is true template output.
3. **Merge-body files** → strip any hand-added `Ignore` (or merge-protected *stub*) that was only there to make the reference app run. Keep an ignore **only** where it deliberately mirrors real developer bespoke code.
4. **Never leave a hand-added `Ignore` on a template-owned region** — it silently opts that region out of regeneration, so the diff shows the hand-craft instead of the template's real output.

A body the template itself emits with `Body = Mode.Ignore` is never rewritten over the reference app's hand-written code, so Phase 1 cannot check it. Check its first-run body, usually the stub, in the Phase 2 from-scratch run.

### Coverage — the spanning set and per-type topology

Cover the **spanning set** of distinct shapes the technology can produce — not just the simplest case. Module-type-specific scenario intuition matters — e.g. **eventing modules must be tested with both a single app (publisher + subscriber in-process) AND two apps (publisher in A, subscriber in B)**, because a pattern that holds in one can break in the other (shared contracts, explicit topology, subscriber-side discovery surface only across a process boundary).

### Comprehensive test models — not toy ones

When a module generates **from domain / DTO / entity models**, the *model's structure* is itself a coverage axis. Do not model one flat DTO/entity with 2–3 scalar properties and call it done — deliberately exercise the OOP aspects the target domain actually has:

- **composition** and **aggregates** (independent relationships)
- **inheritance**
- **nullability** and **collections**
- **operations** (where relevant), **parameter lists**, and **parameterized constructors**

A module verified only against a toy model silently fails on the variations developers really use — the same spanning-set failure, at the *model-structure* level (an incremental-mapping bug can easily hide when relationships are never exercised). **Expand to the domain's real shapes, then let the developer cut** what's out of scope — don't start minimal, and don't pad artificially beyond what the domain has.

### App/solution creation tooling

The AI should **attempt to create the app/solution itself** via the MCP creation tools (`create_application` / `create_solution`). Only if those tools are missing or fail should it **ask the user** to perform the step.

===

## 5. How A Module Carries AI Guidance

A module can ship guidance for the AI agents working in the applications that install it. There are two channels, split by what the agent is doing:

- **Designer `AI Settings` → `Rules`** — for an agent *modelling* in a designer: what an element type is for, when to use it, which stereotype it needs. Set them on the designer, element, or association settings (or extension) you own; Intent Architect hands them to the agent with the designer's schema. Mechanics: `add-designer-extension`.
- **A generated `SKILL.md`** — for an agent *writing code* against the module's output: how to fill in a generated handler, what a convention expects. A template in the module generates it into the consuming application, so it is versioned and updated with the module.

A new module must **never register an AI task provider**. AI task providers are deprecated; guidance goes through the two channels above only.

That leaves an **accepted gap**. A task provider could flag, after each Software Factory run, the specific files still needing hand-written code ("this handler still needs implementing"). Nothing replaces that proactive flag, and none is planned: rules and skills are read when an agent looks for guidance, not pushed when generation leaves work behind. Accept that, and word the guidance so an agent that reads it can find the work itself:

- **Rules are conditional on generation.** Write "once the Software Factory has generated the handler for this command, write its body: …", not "implement the handler" — the rule is read while modelling, before any code exists.
- **Skills say how to find pending work.** For example: "find the handlers still to write by their `throw new NotImplementedException(...)` stub, then …". That is what a generated stub carries until a developer writes the body (`file-builder-expert` Musts #11 shows how a template emits it). Do not point at `// IntentInitialGen`: it sits in the template's source only and never reaches the generated file.
