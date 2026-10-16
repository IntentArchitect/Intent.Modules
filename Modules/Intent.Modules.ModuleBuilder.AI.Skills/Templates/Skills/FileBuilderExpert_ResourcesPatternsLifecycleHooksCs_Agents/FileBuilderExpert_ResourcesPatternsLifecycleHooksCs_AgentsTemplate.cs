using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.FileBuilderExpert_ResourcesPatternsLifecycleHooksCs_Agents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FileBuilderExpert_ResourcesPatternsLifecycleHooksCs_AgentsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return """"""
// PATTERN: Build Lifecycle Hooks — OnBuild / AfterBuild
// =======================================================
// KEY RULES:
//   - TWO PHASES. Every OnBuild callback of every template runs first; only then does the AfterBuild
//     phase start. An AfterBuild callback therefore always sees all OnBuild work, whatever the numbers.
//   - Priority orders callbacks WITHIN one phase: lower integer = runs FIRST (default 0). OnBuild 1000
//     still runs before AfterBuild 0.
//   - OnBuild:    structural composition (add members, modify class shape).
//   - AfterBuild: final reconciliation (cross-template wiring, late additions, clean-up).
//   - Register callbacks in the constructor (your own template) or in a factory extension's
//     OnAfterTemplateRegistrations (another module's template). A callback registered from inside a
//     running callback is queued into the current phase and ordered with the rest. Only an OnBuild
//     registered after the OnBuild phase has ended throws "This file has already been built".
//   - Callbacks run only for templates whose CanRunTemplate() is true when the phase starts. A template
//     that is switched off runs none of its callbacks, including ones a factory extension added.
//   - FileBuilderHelper (Intent.Modules.Common) is the authority on sort order within a phase:
//       priority → template-type-name → template-id → model-id → creation order.
//     Do NOT rely on implicit ordering when two templates touch the same file.
//
// PRIORITY BAND CONVENTION (use the same bands in each phase):
//   Band 0    — Core scaffolding:    the owning template creates the primary structure.
//   Band 100  — Enrichment:          same-module cross-cutting additions (logging, attributes).
//   Band 500  — Extension:           factory extensions from other modules enrich the file.
//   Band 1000 — Final / reconcile:   clean-up, interface wiring, and ANY logic that calls
//                                    FindMethod / FindClass on elements from another template.
//
// THE FIND RULE:
//   When Template B needs to locate an element created by Template A (via FindMethod,
//   FindClass, FindStatement, etc.), B's callback must run after A's:
//     - same phase:      B's priority must be strictly HIGHER than A's;
//     - different phase: B in AfterBuild always sees what A added in OnBuild.
//   Rationale: within a phase FileBuilderHelper executes callbacks in ascending priority order. If B's
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
        // Registered in the constructor; runs in the OnBuild phase.
        var cls = file.Classes.First();
        cls.AddMethod("void", "Execute", method =>
        {
            method.AddStatement("// primary logic generated here");
        });
    })
    .AfterBuild(file =>
    {
        // Runs after ALL OnBuild callbacks across all templates have completed.
        var cls = file.Classes.First();
        var method = cls.FindMethod("Execute");
        method?.AddStatement("// reconciliation step added by AfterBuild");
    });

// --- SIDE-BY-SIDE EXAMPLE: Template A creates, Template B finds ---
//
// Template A (OnBuild, Band 0 — Core): owns the file, adds a private helper method.
// Template B (AfterBuild, Band 1000 — Final): finds that method and injects a log statement.
// Because B runs in AfterBuild, A's OnBuild has already run whatever B's number is. Had B used
// OnBuild instead, it would need a priority > 0.

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
        // AfterBuild: every OnBuild (any priority) has already executed.
        var method = file.Classes.First().FindMethod("ProcessOrder");
        if (method == null)
            throw new InvalidOperationException("ProcessOrder not found — Template A did not add it.");

        method.Statements.First().InsertAbove(
            new CSharpStatement("_logger.LogDebug(\"Processing order {Id}\", order.Id);"));
    }, 1000);   // Band 1000 — orders it after other AfterBuild callbacks

// --- PRIORITY BANDS REFERENCE ---

CSharpFile
    .OnBuild(file =>
    {
        // OnBuild Band 0 — Core: structural scaffolding owned by this template.
        file.Classes.First().AddField("ILogger<OrderService>", "_logger", f => f.PrivateReadOnly());
    }, 0)
    .OnBuild(file =>
    {
        // OnBuild Band 100 — Enrichment: same-module cross-cutting additions.
        file.Classes.First().AddMethod("void", "LogStartup",
            m => m.AddStatement("_logger.LogInformation(\"Started\");"));
    }, 100)
    .AfterBuild(file =>
    {
        // AfterBuild Band 500 — Extension (a separate scale: runs after every OnBuild above).
        file.Classes.First().AddAttribute("GeneratedByExtension");
    }, 500)
    .AfterBuild(file =>
    {
        // AfterBuild Band 1000 — Final: reconciliation, FindMethod/FindClass across templates.
        var cls = file.Classes.First();
        if (cls.HasMetadata("requires-disposal"))
        {
            cls.ImplementsInterface("IDisposable");
            cls.AddMethod("void", "Dispose", m => m.AddStatement("// cleanup"));
        }
    }, 1000);

// --- FACTORY EXTENSION PATTERN ---
// A factory extension calls OnBuild/AfterBuild on templates it does NOT own. Derive your own from
// FactoryExtensionBase (Intent.Modules.Common.Plugins) — never from CSharpFileBuilderFactoryExtension,
// which is the SDK's own extension, not a base class. Register in OnAfterTemplateRegistrations.

public class MyFactoryExtension : FactoryExtensionBase
{
    public override string Id => "My.Module.MyFactoryExtension";

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        // A per-model template has one instance per model: iterate FindTemplateInstances.
        // FindTemplateInstance (singular) throws when more than one instance matches, so keep it
        // for a template that only ever has one instance.
        foreach (var template in application.FindTemplateInstances<ICSharpFileBuilderTemplate>("My.Other.Module.ServiceTemplate"))
        {
            template.CSharpFile
                .AfterBuild(file =>
                {
                    // Enrich another module's template after every OnBuild has finished.
                    var cls = file.Classes.First();
                    cls.AddAttribute("GeneratedByExtension");
                }, 500);  // mid-range: after the owner's earlier AfterBuild work, before final clean-up
        }
    }
}

// --- FINDING ELEMENTS AFTER BUILD ---

// FindMethod checks only this class's own Methods and returns the FIRST match — it is not recursive
// (no nested types, no statement tree). For overloads use file.Classes.First().Methods.Where(m => m.Name == "Execute").
var target = file.Classes.First().FindMethod("Execute");

// FindStatement walks into nested blocks (if / foreach / try ...), but NOT into an assignment's
// right-hand side and NOT into switch-case bodies. For "options.X = [ ... ]", find the assignment first,
// then search its Rhs — see builder-patterns.md § "Editing Collection Items and Switch-Case Statements".
var assignment = method.FindStatement<CSharpAssignmentStatement>(s => s.Lhs.GetText("") == "var result");

// Match statements another template added by a metadata tag it set, never by rendered text:
var unhandled = method.FindStatement<CSharpCatchBlock>(c => c.HasMetadata("my-module:unhandled-catch"));

// InsertAbove / InsertBelow for positional insertion inside an existing method:
existingStatement.InsertAbove(new CSharpStatement("// header comment"));
existingStatement.InsertBelow(new CSharpStatement("// footer comment"));

"""""";
        }
    }
}
