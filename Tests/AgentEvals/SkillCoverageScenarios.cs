namespace AgentEvals;

// ---------------------------------------------------------------------------------------------
// Layer 3 - guidance actually used: coverage of the AI.Guidelines skill set, for comparing two versions of it
// ---------------------------------------------------------------------------------------------
//
// C1-C25 were written by an independent case writer from the 1.0.4-pre.1 AI.Skills skills only, to show whether
// a revised skill set gives the same results (C:\Dev\.ai-reports\2026-10-21-skill-trim-eval-cases.md). Each
// lists the skills that carry its answer in both sets - old name first, then the skill it was folded into - so
// one case scores either set. Run them against each set's generated output with --generated.
//
// C1-C4, C9-C13 and C22-C25 covered skills that Intent Architect 5.4 ships built in, so they are no longer
// tested from here; their ids stay retired. C5 and C14 now count only module-building-strategies, the one
// skill still shipped from here that carries their answer.

/// <summary>A how-to question whose answer an always-on instruction carries, so finding it is informational.</summary>
public abstract class InstructionRoute : RoutingScenario
{
    protected override bool GuidanceLeavesATrace => false;
}

// Cluster A - designer extensions

/// <summary>C5 - a package's stereotypes ship only when the package is set to Include in Module.</summary>
public sealed class IncludeInModuleRoute : RoutingScenario
{
    public override string Id => "C5";

    public override string Title => "Stereotype ships nowhere";

    // A plan, not a bug hunt: the fixture has no such package, and an agent asked to find the fault searches for it.
    public override string Prompt =>
        "I'm about to add a new package to SampleModule's Module Builder designer to hold a 'Cacheable' stereotype definition " +
        "for Domain classes. It isn't in the repo yet. Apart from defining the stereotype itself, what do I have to set on that " +
        "package so the stereotype actually ships to consuming apps' Domain designer?" + AnswerOnly;

    protected override string[] Skills => ["module-building-strategies"];

    protected override string[][] AnswerMarkers => [["include in module"]];

    protected override string Expectation => "the package's Include in Module setting";
}

// Cluster B - element icons vs package icon

/// <summary>C6 - a state-dependent element-type icon uses Icon Function, and returning null falls back to the default.</summary>
public sealed class IconFunctionRoute : RoutingScenario
{
    public override string Id => "C6";

    public override string Title => "Element-type icon that depends on state";

    public override string Prompt =>
        "A custom element type defined in SampleModule's designer should show a padlock icon when its 'Is Locked' property is " +
        "ticked, and its normal icon otherwise. How do I set that up?" + AnswerOnly;

    protected override string[] Skills => ["module-element-icons", "module-svg-icon"];

    protected override string[][] AnswerMarkers => [["icon function"], ["null"]];

    protected override string Expectation => "an Icon Function that returns null for the default icon";
}

/// <summary>C7 - a stereotype's badge that depends on a property is its definition's displayIconFunction.</summary>
public sealed class StereotypeBadgeFunctionRoute : RoutingScenario
{
    public override string Id => "C7";

    public override string Title => "Stereotype badge only when a property is set";

    public override string Prompt =>
        "SampleModule's 'Cacheable' stereotype should show its icon badge only on Domain classes where its 'Enabled' property is " +
        "ticked, and no badge otherwise. Where do I configure that?" + AnswerOnly;

    protected override string[] Skills => ["module-element-icons", "module-svg-icon"];

    protected override string[][] AnswerMarkers => [["displayiconfunction", "display icon function"]];

    protected override string Expectation => "the stereotype definition's displayIconFunction";
}

/// <summary>
/// C8 - the plan for a bespoke stereotype icon: a script computes the base64 into a file passed to run_designer_script
/// through includedScriptPaths, so the payload never passes through the agent. A plan, because applying it needs the
/// designer API, which the MCP stub cannot execute.
/// </summary>
public sealed class StereotypeIconPlanRoute : RoutingScenario
{
    private const string Stereotype = "SampleModule/Intent.Metadata/Module Builder/SampleModule/Elements/Cacheable.xml";

    private const string ApprovedPath = "../approved/cacheable.svg";

    public override string Id => "C8";

    public override string Title => "Plan for a bespoke stereotype icon without the payload passing through the agent";

    public override string Prompt =>
        $"The approved SVG for SampleModule's 'Cacheable' stereotype is at {ApprovedPath}, outside this repository. Before anything " +
        "is changed: how exactly will the icon get onto the stereotype? Outline the steps and the tool calls; don't change any files.";

    protected override string[] Skills => ["module-element-icons", "module-svg-icon"];

    protected override string[][] AnswerMarkers => [["includedscriptpaths"], ["base64"]];

    protected override string Expectation => "a script writes the base64 into a file passed through includedScriptPaths";

    public override void Arrange(string root)
    {
        File.WriteAllText(Path.Combine(root, Stereotype),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<class id=\"5b0c1e2d-0000-4000-8000-0000cac4ab1e\" type=\"Stereotype Definition\">\n" +
            "  <name>Cacheable</name>\n" +
            "</class>\n");
        var approved = Path.GetFullPath(Path.Combine(root, ApprovedPath));
        Directory.CreateDirectory(Path.GetDirectoryName(approved)!);
        File.WriteAllText(approved, Icon.ApprovedSvg);
    }

    public override IEnumerable<Check> Checks(Evidence evidence) =>
        base.Checks(evidence).Append(Icon.PayloadKeptOut(evidence, Icon.ApprovedSvg));
}

// Cluster E - strategies

/// <summary>C14 - a hand-written body is emitted with the typed CSharpIntentManagedAttribute and a NotImplementedException stub.</summary>
public sealed class IgnoredBodyRoute : RoutingScenario
{
    public override string Id => "C14";

    public override string Title => "An \"implement here\" body";

    // Asks for the CSharpFile builder calls, which is what the skill's description routes on. Naming only the mode is
    // what the always-on known-build-gotchas teaches, so the typed attribute is the marker.
    public override string Prompt =>
        "I'm writing a new C# template for SampleModule (not in the repo yet). Its TemplatePartial builds a handler class with the " +
        "CSharpFile builder and adds a `Handle` method from the model. The developer writes that method's body by hand: the " +
        "signature should keep following the model, and the body must survive later Software Factory runs. Show me the builder " +
        "calls on the method that set this up." + AnswerOnly;

    protected override string[] Skills => ["module-building-strategies"];

    protected override string[][] AnswerMarkers => [["csharpintentmanagedattribute"], ["notimplementedexception"]];

    protected override string Expectation => "CSharpIntentManagedAttribute.IgnoreBody() with a NotImplementedException stub";
}

/// <summary>C15 - the MediatR module's AI task provider is a pattern not to copy.</summary>
public sealed class AiTaskProviderNotCopiedRoute : RoutingScenario
{
    public override string Id => "C15";

    public override string Title => "Flagging handlers still to write";

    public override string Prompt =>
        "Intent.Modules.NET's MediatR module flags generated handlers that still need writing after each Software Factory run. " +
        "I want SampleModule to flag its generated handlers the same way. How do I set that up?" + AnswerOnly;

    protected override string[] Skills => ["module-building-strategies"];

    protected override string[][] AnswerMarkers => [["deprecated"], ["notimplementedexception", "skill.md"]];

    protected override string Expectation => "the AI task provider is deprecated; flag with a stub or a skill instead";
}

// Cluster F - build gotchas and version raises, carried by instructions

/// <summary>
/// C16 - Intent.Modules.Common 3.10.x needs Intent.SoftwareFactory.SDK 3.13.0, which needs Intent Architect 4.6.0: links
/// only known-build-gotchas carries (the 3.11.0 -> 3.14.0 one is also in a constant skill).
/// </summary>
public sealed class Nu1605ChainRoute : InstructionRoute
{
    public override string Id => "C16";

    public override string Title => "The NU1605 chain: Common 3.10.x needs SDK 3.13.0 and Intent Architect 4.6.0";

    public override string Prompt =>
        "In SampleModule I'm raising the `Intent.Modules.Common` package reference from Module Builder's 3.7.2 to a 3.10.x " +
        "release, because a designer package I need depends on it (the template using it is " +
        "SampleModule/Templates/Sample/SampleTemplatePartial.cs). Before I restore: which other package reference will restore " +
        "make me raise next, to what version, and what is the oldest Intent Architect version that can then run the module?" + AnswerOnly;

    protected override string[] Skills => ["known-build-gotchas"];

    protected override string[][] AnswerMarkers => [["3.13.0"], ["4.6.0"]];

    protected override string Expectation => "the SDK raised to 3.13.0, which needs Intent Architect 4.6.0";
}

/// <summary>C17 - a NuGet dependency added in AfterBuild is never collected; add it in the constructor or OnBuild.</summary>
public sealed class NugetTooLateRoute : InstructionRoute
{
    public override string Id => "C17";

    public override string Title => "NuGet dependency added too late";

    public override string Prompt =>
        "SampleModule/Templates/Sample/SampleTemplatePartial.cs calls `AddNugetDependency(\"Polly\", \"8.4.0\")` inside an " +
        "`AfterBuild` callback, because whether it's needed depends on another template. The package never appears in the " +
        "consuming application's .csproj, and there's no error. Why?" + AnswerOnly;

    protected override string[] Skills => ["known-build-gotchas"];

    protected override string[][] AnswerMarkers => [["onbuild"], ["collect"]];

    protected override string Expectation => "packages are collected before AfterBuild; add it in OnBuild";
}

/// <summary>C18 - inside a build callback an end-user error is an ElementException, and its message is plain text.</summary>
public sealed class ElementExceptionInCallbackRoute : InstructionRoute
{
    public override string Id => "C18";

    public override string Title => "End-user error inside a build callback";

    public override string Prompt =>
        "In an OnBuild callback in SampleModule/Templates/Sample/SampleTemplatePartial.cs I throw " +
        "`new FriendlyException(\"**Retry Count** must be positive\")` when a class's value is invalid, but users only see " +
        "'An unexpected error occurred while building…'. How should I raise it?" + AnswerOnly;

    protected override string[] Skills => ["exception-guidelines"];

    protected override string[][] AnswerMarkers => [["elementexception"], ["markdown", "not rendered", "plain text", "plain prose"]];

    protected override string Expectation => "an ElementException, without Markdown";
}

/// <summary>C19 - a FriendlyException needs Intent.SoftwareFactory.SDK 3.9.0, above Module Builder's default.</summary>
public sealed class FriendlyExceptionSdkRoute : InstructionRoute
{
    public override string Id => "C19";

    public override string Title => "FriendlyException needs an SDK raise";

    public override string Prompt =>
        "SampleModule should stop the Software Factory with a readable error when the Intent.EntityFrameworkCore module isn't " +
        "installed. The check goes in SampleModule/Templates/Sample/SampleTemplatePartial.cs's registration. What should I throw, " +
        "and does a module scaffolded with Module Builder's defaults need anything changed first?" + AnswerOnly;

    protected override string[] Skills => ["exception-guidelines"];

    protected override string[][] AnswerMarkers => [["friendlyexception"], ["3.9.0"]];

    protected override string Expectation => "a FriendlyException, after raising the SDK to 3.9.0";
}

// Cluster G - reworded-only skills, one routing case and top trap each

/// <summary>
/// C20 - TemplateRoles.Domain.Entity.Primary's value is "Domain.Entity". An answer that warns against "Domain.Entity" or
/// lists both strings also carries the marker, so the comparison reads the judge's Steered score as well.
/// </summary>
public sealed class EntityRoleLiteralRoute : RoutingScenario
{
    public override string Id => "C20";

    public override string Title => "Literal for TemplateRoles.Domain.Entity.Primary";

    public override string Prompt =>
        "I'm porting a factory extension into SampleModule that looks up templates by `TemplateRoles.Domain.Entity.Primary`. " +
        "Intent.Modules.Constants isn't on nuget.org. What literal string do I pass instead?" + AnswerOnly;

    protected override string[] Skills => ["intent-module-orchestrator"];

    protected override string[][] AnswerMarkers => [["\"domain.entity\"", "`domain.entity`"]];

    protected override string Expectation => "the literal \"Domain.Entity\"";

    public override string JudgeBrief =>
        "The user asked which literal string replaces the constant TemplateRoles.Domain.Entity.Primary in a template lookup, and " +
        "asked for no file changes. Good behaviour, both must hold: (1) the one literal the answer tells the user to pass is exactly " +
        "\"Domain.Entity\"; (2) it does not recommend \"Domain.Entity.Primary\" - naming it only as the wrong choice is fine. " +
        "Anything else is bad.";
}

/// <summary>C21 - a registration request published from the constructor is dropped; publish it after registration.</summary>
public sealed class RegistrationTooEarlyRoute : RoutingScenario
{
    public override string Id => "C21";

    public override string Title => "Registration request published too early";

    public override string Prompt =>
        "SampleModule's template publishes a `ContainerRegistrationRequest` from its constructor, but the registration never " +
        "shows up in the consuming app's DependencyInjection class, and there's no error. What's wrong?" + AnswerOnly;

    protected override string[] Skills => ["intent-module-orchestrator"];

    protected override string[][] AnswerMarkers => [["aftertemplateregistration", "beforetemplateexecution"]];

    protected override string Expectation => "publish after template registration";
}
