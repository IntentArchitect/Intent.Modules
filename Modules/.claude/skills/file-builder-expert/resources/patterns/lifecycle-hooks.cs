// PATTERN: Build Lifecycle Hooks — OnBuild / AfterBuild
// =======================================================
// KEY RULES:
//   - ALL OnBuild and AfterBuild callbacks MUST be registered during constructor setup.
//     Registering them later causes "file has already been built" exceptions.
//   - Lower priority integer = runs FIRST. Use consistent numeric bands across templates.
//   - OnBuild:    structural composition (add members, modify class shape).
//   - AfterBuild: final reconciliation (cross-template wiring, late additions, clean-up).
//   - FileBuilderHelper.cs is the authority on sort order:
//       priority → template-type-name → template-id → model-id → creation order.
//     Do NOT rely on implicit ordering when two templates touch the same file.
//
// PRIORITY BAND CONVENTION (mandatory for this workspace):
//   Band 0    — Core scaffolding:    the owning template creates the primary structure.
//   Band 100  — Enrichment:          same-module cross-cutting additions (logging, attributes).
//   Band 500  — Extension:           factory extensions from other modules enrich the file.
//   Band 1000 — Final / reconcile:   clean-up, interface wiring, and ANY logic that calls
//                                    FindMethod / FindClass on elements from another template.
//
// THE FIND RULE:
//   When Template B needs to locate an element created by Template A (via FindMethod,
//   FindClass, FindStatement, etc.), Template B's callback MUST use a strictly HIGHER
//   priority number than Template A's callback.
//   Rationale: FileBuilderHelper executes callbacks in ascending priority order. If B's
//   priority ≤ A's priority, A may not have added the element yet when B searches for it.
//
// MUST NOT:
//   Never use implicit priority (omitting the second argument, which defaults to 0) for
//   reconciliation logic that depends on the existence of elements from other modules.
//   Always supply an explicit integer so the ordering is unambiguous.

// --- BASIC ONBUILD ---

CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
    .AddClass("MyService", @class =>
    {
        @class.AddConstructor();  // structural shell added unconditionally
    })
    .OnBuild(file =>
    {
        // Safe to call — build has not started yet; this callback queues during construction.
        var cls = file.Classes.First();
        cls.AddMethod("void", "Execute", method =>
        {
            method.AddStatement("// primary logic generated here");
        });
    })
    .AfterBuild(file =>
    {
        // Runs after ALL OnBuild delegates across all templates have completed.
        var cls = file.Classes.First();
        var method = cls.FindMethod("Execute");
        method?.AddStatement("// reconciliation step added by AfterBuild");
    });

// --- SIDE-BY-SIDE EXAMPLE: Template A creates, Template B finds ---
//
// Template A (Band 0 — Core): owns the file, adds a private helper method.
// Template B (Band 1000 — Final): finds that method and injects a log statement.
// B MUST use priority 1000 (> 0) so A's OnBuild has already run when B searches.

// ── Template A ──────────────────────────────────────────────────────────────
CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
    .AddClass("OrderService", @class =>
    {
        @class.AddConstructor();
    })
    .OnBuild(file =>
    {
        // Band 0: core scaffold — creates the private method that Template B will find.
        file.Classes.First().AddMethod("void", "ProcessOrder", method =>
        {
            method.Private();
            method.AddParameter("Order", "order");
            method.AddStatement("// core processing logic");
        });
    }, 0);   // explicit Band 0

// ── Template B (factory extension or sibling template) ───────────────────────
template.CSharpFile
    .AfterBuild(file =>
    {
        // Band 1000: FindMethod is safe here because all Band 0–500 callbacks
        // have already executed. Never search at priority ≤ the source template.
        var method = file.Classes.First().FindMethod("ProcessOrder");
        if (method == null)
            throw new Exception("ProcessOrder not found — check that Template A runs at a lower priority.");

        method.Statements.First().InsertAbove(
            new CSharpStatement("_logger.LogDebug(\"Processing order {Id}\", order.Id);"));
    }, 1000);   // Band 1000 — MUST be > Template A's priority

// --- PRIORITY BANDS REFERENCE ---

CSharpFile
    .OnBuild(file =>
    {
        // Band 0 — Core: structural scaffolding owned by this template.
        file.Classes.First().AddProperty("ILogger<OrderService>", "_logger", p => p.PrivateReadOnly());
    }, 0)
    .OnBuild(file =>
    {
        // Band 100 — Enrichment: same-module cross-cutting additions.
        file.Classes.First().AddMethod("void", "LogStartup",
            m => m.AddStatement("_logger.LogInformation(\"Started\");"));
    }, 100)
    .AfterBuild(file =>
    {
        // Band 500 — Extension: factory extensions from other modules.
        file.Classes.First().AddAttribute("GeneratedByExtension");
    }, 500)
    .AfterBuild(file =>
    {
        // Band 1000 — Final: reconciliation, FindMethod/FindClass across templates.
        var cls = file.Classes.First();
        if (cls.HasMetadata("requires-disposal"))
        {
            cls.ImplementsInterface("IDisposable");
            cls.AddMethod("void", "Dispose", m => m.AddStatement("// cleanup"));
        }
    }, 1000);

// --- FACTORY EXTENSION PATTERN ---
// Factory extensions (subclasses of CSharpFileBuilderFactoryExtension) call OnBuild/AfterBuild
// on templates they do NOT own. Priority becomes critical here.

// In the factory extension:
template.CSharpFile
    .AfterBuild(file =>
    {
        // Enrich another module's template after its own OnBuild has finished.
        var cls = file.Classes.First();
        cls.AddAttribute("GeneratedByExtension");
    }, 500);  // use a mid-range priority to run after owner's callbacks but before final clean-up

// --- FINDING ELEMENTS AFTER BUILD ---

// FindMethod searches the full statement tree recursively:
var target = file.Classes.First().FindMethod("Execute");

// FindStatement walks into nested blocks:
var assignment = method.FindStatement<CSharpAssignmentStatement>(s => s.Lhs.Text == "var result");

// InsertAbove / InsertBelow for positional insertion inside an existing method:
existingStatement.InsertAbove(new CSharpStatement("// header comment"));
existingStatement.InsertBelow(new CSharpStatement("// footer comment"));
