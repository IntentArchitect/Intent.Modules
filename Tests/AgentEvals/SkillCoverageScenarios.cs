namespace AgentEvals;

// ---------------------------------------------------------------------------------------------
// Layer 3 - guidance actually used: coverage of the AI.Skills set, for comparing two versions of it
// ---------------------------------------------------------------------------------------------
//
// C1-C25 were written by an independent case writer from the 1.0.4-pre.1 skills only, to show whether a
// revised skill set gives the same results (C:\Dev\.ai-reports\2026-10-21-skill-trim-eval-cases.md). Each
// lists the skills that carry its answer in both sets - old name first, then the skill it was folded into - so
// one case scores either set. Run them against each set's generated output with --generated.

/// <summary>A how-to question whose answer an always-on instruction carries, so finding it is informational.</summary>
public abstract class InstructionRoute : RoutingScenario
{
    protected override bool GuidanceLeavesATrace => false;
}

// Cluster A - association types and designer extensions

/// <summary>C1 - an association type is drawn through an Association Creation Option on the source's Element Extension.</summary>
public sealed class AssociationCreationOptionRoute : RoutingScenario
{
    public override string Id => "C1";

    public override string Title => "Association type: nothing to draw it with";

    public override string Prompt =>
        "I've modelled a new association type in SampleModule's Module Builder designer for drawing from a Command to an " +
        "integration event in the Services designer. Its Designer Settings node already extends the Services designer and the " +
        "Software Factory ran without errors, but when users right-click a Command there is no option to draw it. What is missing?" + AnswerOnly;

    protected override string[] Skills => ["add-association-type", "add-designer-extension"];

    protected override string[][] AnswerMarkers => [["association creation option"], ["element extension"], ["target end", "destination end"]];

    protected override string Expectation => "an Association Creation Option on the source's Element Extension, typed to the target end";
}

/// <summary>C2 - the mapping dialog and the Processing Action trait go on the association's target end.</summary>
public sealed class AssociationMappingRoute : RoutingScenario
{
    public override string Id => "C2";

    public override string Title => "Association type: mapping dialog and traits";

    public override string Prompt =>
        "Users of SampleModule's new 'Publish Integration Event' association should be able to open a mapping dialog from it, " +
        "and the module's mapping settings need to tell which end of it is mappable. How do I configure the association?" + AnswerOnly;

    protected override string[] Skills => ["add-association-type", "add-designer-extension"];

    protected override string[][] AnswerMarkers => [["target end", "destination end"], ["processing action"]];

    protected override string Expectation => "the mapping option and the [Processing Action] trait on the target end";
}

/// <summary>C3 - a context-menu option on a foreign element needs Type Reference Extension Settings and an Element Creation Option.</summary>
public sealed class ForeignContextMenuRoute : RoutingScenario
{
    public override string Id => "C3";

    public override string Title => "Context-menu option on a foreign element";

    public override string Prompt =>
        "SampleModule should add a 'New Audit Rule' option to the context menu of the Domain designer's Class element. Class " +
        "belongs to Intent.Modelers.Domain, not to SampleModule. What do I model in the Module Builder designer?" + AnswerOnly;

    protected override string[] Skills => ["add-designer-extension"];

    // The SKILL.md names the option's [context menu] parent; "Element Creation Option" is only in its workflow resource.
    protected override string[][] AnswerMarkers => [["type reference extension settings"], ["element creation option", "[context menu]"]];

    protected override string Expectation => "an Element Extension with Type Reference Extension Settings and an Element Creation Option";
}

/// <summary>C4 - AI Settings Rules ship only from element types; on a Package Extension they are dropped.</summary>
public sealed class AiRulesOnPackageExtensionRoute : RoutingScenario
{
    public override string Id => "C4";

    public override string Title => "AI rules on a Package Extension are dropped";

    public override string Prompt =>
        "SampleModule ships guidance for AI agents modelling in the Services designer: I applied AI Settings to its Package " +
        "Extension for the Services package and wrote the guidance in Rules. After regenerating, agents in consuming apps never " +
        "get it. Why, and where should it go instead?" + AnswerOnly;

    protected override string[] Skills => ["add-designer-extension"];

    protected override string[][] AnswerMarkers =>
        [["element extension", "designer settings", "element settings"],
         ["dropped", "not emitted", "isn't emitted", "not carried", "isn't carried", "not copied", "isn't copied", "silently", "only from"]];

    protected override string Expectation => "Rules on a Package Extension are dropped; they ship from element types";
}

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

    protected override string[] Skills => ["add-designer-extension", "add-association-type", "module-building-strategies", "module-versioning"];

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

// Cluster C - mapping

/// <summary>C9 - a resolver that returns null breaks the next one; it hands on with next?.Invoke.</summary>
public sealed class ResolverReturnsNullRoute : RoutingScenario
{
    public override string Id => "C9";

    public override string Title => "Resolver returning null breaks the next one";

    public override string Prompt =>
        "After I added SampleModule's custom mapping resolver, another installed module's resolver started throwing a " +
        "NullReferenceException where it calls `next(mappingModel)`. Mine returns `null` for mapping types it doesn't handle. " +
        "What's wrong, and how should mine look?" + AnswerOnly;

    protected override string[] Skills => ["intent-mapping-architect", "file-builder-expert"];

    protected override string[][] AnswerMarkers => [["next?.invoke"]];

    protected override string Expectation => "hand unhandled nodes on through next?.Invoke";
}

/// <summary>C10 - the two-argument resolver needs Intent.Modules.Common.CSharp 3.9.2 and Intent.Modules.Common 3.7.3.</summary>
public sealed class ResolverVersionsRoute : RoutingScenario
{
    public override string Id => "C10";

    public override string Title => "Versions for the two-argument resolver";

    public override string Prompt =>
        "SampleModule still has Module Builder's default package versions. I've written a mapping resolver that implements " +
        "`ResolveMappings(MappingModel mappingModel, MappingTypeResolverDelegate next)` and registered it with " +
        "`AddMappingResolver(resolver, priority: -100)`. The build fails with CS0246. What do I need to change?" + AnswerOnly;

    protected override string[] Skills => ["intent-mapping-architect", "file-builder-expert"];

    protected override string[][] AnswerMarkers => [["3.9.2"], ["3.7.3"]];

    protected override string Expectation => "raise Common.CSharp to 3.9.2 and Common to 3.7.3";
}

/// <summary>C11 - a lower (negative) priority makes a resolver run first.</summary>
public sealed class ResolverPriorityRoute : RoutingScenario
{
    public override string Id => "C11";

    public override string Title => "Making a resolver run first";

    public override string Prompt =>
        "Another installed module's mapping resolver already claims the 'Update Entity Mapping' nodes that SampleModule's " +
        "resolver should handle, so mine never gets a chance. How do I make mine run first?" + AnswerOnly;

    protected override string[] Skills => ["intent-mapping-architect", "file-builder-expert"];

    // Intent's own resolvers go down to -10, so "well below", e.g. -100. "-10" and "negative" alone are the deleted
    // interactions skill's strategy priority, which a removed-skill run quoted.
    protected override string[][] AnswerMarkers => [["-100", "below -10", "lower than -10"]];

    protected override string Expectation => "register well below Intent's own -10, e.g. -100";
}

// Cluster D - debugging

/// <summary>C12 - Logging.Log has Debug/Info/Warning/Failure, no Error, and no message placeholders.</summary>
public sealed class LoggingLevelsRoute : RoutingScenario
{
    public override string Id => "C12";

    public override string Title => "Logging levels";

    public override string Prompt =>
        "I added `Logging.Log.Error(\"Retry count is {Count}\", model.RetryCount)` to SampleModule's template to trace a value, " +
        "and it doesn't compile. How should I log it?" + AnswerOnly;

    protected override string[] Skills => ["module-debugging", "file-builder-expert"];

    protected override string[][] AnswerMarkers =>
        [["failure"], ["$\"", "interpolat", "concatenat", "string.format", "build the string", "string yourself"]];

    protected override string Expectation => "Logging.Log.Failure with the message built first";
}

/// <summary>C13 - module-task logs go to the module-tasks folder, not the software-factory one.</summary>
public sealed class ModuleTaskLogsRoute : RoutingScenario
{
    public override string Id => "C13";

    public override string Title => "Where module-task logs go";

    public override string Prompt =>
        "`Logging.Log.Debug` lines I added to SampleModule code that runs as a module task, not during a Software Factory run, " +
        "never show up in the Software Factory log files. Where do they go?" + AnswerOnly;

    protected override string[] Skills => ["module-debugging", "file-builder-expert"];

    protected override string[][] AnswerMarkers => [["module-tasks"]];

    protected override string Expectation => "the module-tasks log folder";
}

// Cluster E - file-builder-expert and strategies

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

    protected override string[] Skills => ["file-builder-expert", "module-building-strategies"];

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

/// <summary>C22 - an architecture template's module version is its Component Module element's, not the .iatspec's.</summary>
public sealed class ComponentModuleVersionRoute : RoutingScenario
{
    public override string Id => "C22";

    public override string Title => "Propagating a version to an architecture template";

    public override string Prompt =>
        "SampleModule 1.1.0 is now published. Our architecture template pins SampleModule 1.0.0. I edited the version in its " +
        "metadata.iatspec, but the next Software Factory run put 1.0.0 back. Where should I change it?" + AnswerOnly;

    protected override string[] Skills => ["module-versioning", "architecture-templates"];

    protected override string[][] AnswerMarkers => [["component module"]];

    protected override string Expectation => "the Component Module element in the Application Template Builder designer";
}

/// <summary>C23 - a module README has the canonical sections, among them What This Module Generates and Related Modules.</summary>
public sealed class ModuleReadmeSectionsRoute : RoutingScenario
{
    public override string Id => "C23";

    public override string Title => "A module README's canonical sections";

    public override string Prompt =>
        "SampleModule's docs/README.md is only a title and one line. Bring it up to our standard layout for a module README: " +
        "which H2 sections should it have, in which order, and which common README sections should it leave out? " +
        "Just list them; don't change any files.";

    protected override string[] Skills => ["module-docs"];

    protected override string[][] AnswerMarkers => [["what this module generates"], ["related modules"]];

    protected override string Expectation => "the canonical sections, What This Module Generates and Related Modules among them";
}

/// <summary>C24 - consumers' metadata is fixed on update by a Version Migration using IPersistenceLoader.</summary>
public sealed class VersionMigrationRoute : RoutingScenario
{
    public override string Id => "C24";

    public override string Title => "Fixing consumers' metadata on update";

    public override string Prompt =>
        "SampleModule's next version renames its 'Retry Policy' stereotype. Applications that installed 1.0.1 still have the old " +
        "stereotype in their own metadata and need it fixed when they update. How do I set that up?" + AnswerOnly;

    protected override string[] Skills => ["add-module-migration"];

    protected override string[][] AnswerMarkers => [["version migration"], ["ipersistenceloader"]];

    protected override string Expectation => "a Version Migration through IPersistenceLoader";
}

/// <summary>C25 - an architecture template seeds designer content through an .installation.config in resources/.</summary>
public sealed class SeedMetadataRoute : RoutingScenario
{
    public override string Id => "C25";

    public override string Title => "Seeding designer content for new apps";

    public override string Prompt =>
        "New applications created from our architecture template should start with a 'Domain' package already present in the " +
        "Domain designer. Where does that seed content go, and how do I produce it?" + AnswerOnly;

    protected override string[] Skills => ["architecture-templates"];

    protected override string[][] AnswerMarkers => [["installation.config"], ["resources/", "resources folder", "`resources`"]];

    protected override string Expectation => "an .installation.config under the template's resources/";
}
