---
name: module-building-strategies
description: "The accumulated strategic playbook of judgment calls for designing an Intent Architect module — decomposition (root/bridging/common), template vs factory-extension choice, file cardinality, managed modes, setting-vs-stereotype, convention-vs-explicit, and two-phase verification. USE ONLY WHEN facing a design decision point while building or extending a module. DO NOT USE FOR the mechanical how-to of a specific construct (see add-module-skill-template, add-designer-extension, add-association-type) — this is the judgment layer above those."
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleBuildingStrategies_SkillMd_Agents
contentHash: D2E9CF2B96C06C9259A0C7CFD0DAC47E2378190674D0951C4A03C3A2D60DC745
---
# Module Building Strategies

This skill captures *the judgment* — the strategic instincts behind building Intent Architect modules, distinct from the mechanical steps of scaffolding one. Load it whenever you face a design decision, not just a mechanical step.

> **Governing philosophy:** Intent does not dictate how things should work. It gives the developer maximum flexibility to accomplish what's needed, **built on top of industry best practices**. Every strategy below serves that philosophy.

===

## Discover Before You Decide — Module Reconnaissance

This skill holds the **WHY** (strategy/judgment). It deliberately does **not** enumerate per-module **WHAT** — which base class the MassTransit consumer uses, what `Intent.Common.CSharp` offers, which request types exist, what version of a package to use. Those facts are **discoverable and volatile**: documenting them here would rot, and the code/platform already hold the current truth.

- *Rule: never assume a per-module fact from memory or from a skill. Discover it fresh each build, then interpret it through the strategy below** (code shows *what* a module does, rarely *why*).

Read order — cheapest and most authoritative first:

1. **`search_docs`** — IA features, designers, attributes, code-management, module concepts.
2. **`search_available_modules` / `list_installed_modules`** — what's installed/available.
3. **The reference module's own source** — `imodspec` (dependencies, interoperability, version), template registrations (roles, base classes, file cardinality), `NugetPackages.cs` (packages + conditions), factory extensions (what they enrich, which broadcast requests they publish/handle).
4. **The reference app's actual generated output** — ground truth for what the ecosystem already produces; don't re-generate what's already provided.
5. **`find_designer_elements` / `get_designer_element_details`** — element types, stereotypes, and settings that drive generation.

> This protocol is the durable replacement for static per-type "playbooks": it stays current automatically, costs no developer time, and scales to any module.

### Offload discovery to keep the orchestrator lean

Reconnaissance and research are context-heavy and read-only — ideal to **delegate to a sub-agent** (if your harness supports background/sub-agent dispatch) so the main conversation's context stays lean for the work only it can do (designer / SF / model mutation). Hand it a focused discovery brief and take back the distilled findings; if no sub-agent mechanism is available, do the discovery inline.

### Resolve conflicts at the authoritative source

When two modules **disagree** about how a stereotype (or property) is used — one reads it correctly, another (often a legacy or copied module) reads the wrong property — do **not** arbitrate by copying whichever sibling "looks right." Trace back to the **authoritative source: the stereotype's *definition*** (where it is declared), and let that decide which usage is correct. That trace is also what exposes the real culprit — the discrepancy usually points at a bug in a *different* module than the one you were changing. (This is why blind sibling-mimicry is unreliable: the sibling may be the one that's wrong.)

===

## 1. Module Architecture & Decomposition

### Vertical slices — decompose the *work* before the modules

For a large PRD, don't build everything at once. Decompose the work into **vertical slices** — each a *feature* the developer can model and see work end-to-end, which may span **several modules** (e.g. one module for the designer elements, another for code extensibility, another for a different codegen aspect). A slice is the tracking layer **above** the per-module Attack Plan; the units nest:

`PRD → vertical slice (a feature) → spanning set (distinct shapes within it) → increment (one SF cycle)`

Cut a slice so it is **independently valuable and independently testable** — it carries its own acceptance check. Build one slice to green, test it whole, then move to the next; apply the module-decomposition judgment below within each slice.

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

- *AI changes the economics.** When AI can create and maintain modules, that overhead largely dissolves and the justification falls away. Therefore:
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

    - **`Mode.Fully`** — Intent fully manages the file; Intent owns it end-to-end.
    - **`Mode.Merge`** — Intent and the developer **coexist** in that space. Intent generates initial code and opens it for the developer; on later changes, Intent re-detects the regions *it* authored and re-mutates only those, leaving developer code intact. Typical example: command/query handler **bodies**.
    - **`Mode.Ignore`** — **opts out of code automation entirely**; the developer owns and is responsible for that space. It is **not** a generation strategy for module-authored output.
- *Decision rule:** anticipate the developer will want to make changes in a space → **merge**. Otherwise → **fully**. **Ignore is off the table** for module output — choosing it means giving up automation.

===

## 3. Design-Time Configuration

> First distinction: **design-time** configuration (drives generation) vs **runtime** configuration (connection strings, names — belongs in appsettings). The rules below are about *design-time* config only.

### Setting vs stereotype vs association

    - **Module setting** → **application-wide** behavior/rule. Its scope is the whole app, regardless of how anything is modeled.
    - **Stereotype** → **fine-grained, element-level** configuration, often used to **override or specialize a global setting** on specific elements. (Configuration is only one use of stereotypes, not their whole purpose.)
    - **Associations have nothing to do with configuration** — they are a structural modeling mechanism. Never reach for an association to capture a setting.

The common shape is **global default (setting) + per-element override (stereotype)**. Canonical example — **multitenancy**: a *module setting* picks the global isolation level (database-level = one DB per tenant, app-wide; vs entity-level = one shared DB); when entity-level, a *stereotype* on each entity marks it multitenant and gives it a discriminator.

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

- *Phase 1 — reproduce on the reference app.** The reference app first proves the *architecture* works before you automate it. Then install the module *on top of that same reference architecture* and confirm the module outputs **exactly what the hand-written code already has**. This validates that the templates reproduce the proven output.
- *Phase 2 — from-scratch test (mandatory, the gap-exposer).** Once Phase 1 is 100% confirmed, build a **brand-new app**, install the module **with no code pre-written**, and confirm Intent generates everything correctly and fully wired. This is the **only** test that exposes a forgotten wiring step — as a compile error, a runtime error, or "the code just looks wrong." Phase 1 *cannot* catch this because the code was already there.

> This is exactly why "SF shows 0 changes" on the reference app proves nothing on its own — the code was already present. The from-scratch run is the real proof.

### The regeneration-baseline gate (Phase 1 precondition)

Phase 1's comparison is only valid if SF genuinely regenerates the template-owned regions. Before trusting any staged diff:

1. Identify every file the module's templates will own.
2. **100%-Intent-managed files** → **delete-to-regenerate** so SF produces them fresh and the diff is true template output.
3. **Merge-body files** → strip any hand-added `Ignore` (or merge-protected *stub*) that was only there to make the reference app run. Keep an ignore **only** where it deliberately mirrors real developer bespoke code.
4. **Never leave a hand-added `Ignore` on a template-owned region** — it silently opts that region out of regeneration, so the diff shows the hand-craft instead of the template's real output.

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
