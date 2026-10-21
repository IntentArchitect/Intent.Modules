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

    protected override string[][] AnswerMarkers => [["type reference extension settings"], ["element creation option"]];

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

    public override string Prompt =>
        "I added a new package to SampleModule's Module Builder designer with a 'Cacheable' stereotype definition that targets " +
        "Domain classes. The module builds and installs, but the stereotype can't be applied in a consuming app's Domain designer. " +
        "What did I miss?" + AnswerOnly;

    protected override string[] Skills => ["add-designer-extension", "add-association-type", "module-building-strategies"];

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

/// <summary>
/// C7 - the &lt;icon type=...&gt; entries in .application.config are not where a stereotype's icon goes. Markers cannot
/// capture a "no", so the judge scores the route.
/// </summary>
public sealed class ConfigIconIsNotStereotypeIconRoute : RoutingScenario
{
    public override string Id => "C7";

    public override string Title => "The application config's icon entries are not stereotype icons";

    public override string Prompt =>
        "SampleModule.application.config has `<icon type=\"UrlImagePath\" source=\"data:image/svg+xml;base64,…\" />` entries in it. " +
        "Is that where I set the icon for the 'Cacheable' stereotype SampleModule defines? Answer briefly; don't change any files.";

    protected override string[] Skills => ["module-element-icons", "module-svg-icon"];

    protected override string[][] AnswerMarkers => [];

    protected override string Expectation => "judged";

    public override string JudgeBrief =>
        "The user asked whether the <icon type=\"UrlImagePath\" ...> entries in their module's .application.config are where a " +
        "stereotype's icon is set, and asked for no file changes. Good behaviour, all three must hold: (1) the answer says those " +
        "entries are not where a stereotype's icon goes; (2) it names the stereotype definition in the Module Builder designer as " +
        "the place; (3) it does not tell the user to add or edit an <icon> entry in .application.config. Anything else is bad.";

    public override void Arrange(string root) => Icon.Arrange(root);

    public override IEnumerable<Check> Checks(Evidence evidence) =>
        base.Checks(evidence).Append(Icon.PayloadKeptOut(evidence, Icon.PlaceholderSvg));
}

/// <summary>
/// C8 - a bespoke stereotype icon is applied through run_designer_script with the data URI in an included script file,
/// so the payload never passes through the agent, and neither the application config nor the metadata is hand-edited.
/// </summary>
public sealed class StereotypeIconApplied : Scenario
{
    private const string Stereotype = "SampleModule/Intent.Metadata/Module Builder/SampleModule/Elements/Cacheable.xml";

    private const string ApprovedPath = "../approved/cacheable.svg";

    public override string Id => "C8";

    public override string Title => "Bespoke stereotype icon applied without the payload passing through the agent";

    public override Layer Layer => Layer.Guidance;

    public override string Prompt =>
        $"SampleModule defines a 'Cacheable' stereotype. Give it the custom SVG icon at {ApprovedPath} (outside this repository) " +
        "as its icon in the designer.";

    public override void Arrange(string root)
    {
        Icon.Arrange(root);
        File.WriteAllText(Path.Combine(root, Stereotype),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<class id=\"5b0c1e2d-0000-4000-8000-0000cac4ab1e\" type=\"Stereotype Definition\">\n" +
            "  <name>Cacheable</name>\n" +
            "</class>\n");
        var approved = Path.GetFullPath(Path.Combine(root, ApprovedPath));
        Directory.CreateDirectory(Path.GetDirectoryName(approved)!);
        File.WriteAllText(approved, Icon.ApprovedSvg);
    }

    public override IEnumerable<Check> Checks(Evidence evidence)
    {
        var loaded = evidence.SkillsLoaded(["module-element-icons", "module-svg-icon"]);
        var applied = evidence.Read(Stereotype).Contains($"source=\"{Icon.DataUri(Icon.ApprovedSvg)}\"", StringComparison.Ordinal);
        var included = evidence.DesignerScripts.Count(IncludesScriptFiles);
        var handEdited = evidence.ChangedOutsideDesigner(Sample.Metadata);
        var images = evidence.ChangedFiles.Where(f => f.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToList();

        yield return new Check("Skill loaded", loaded.Count > 0,
            loaded.Count > 0 ? $"loaded {string.Join(", ", loaded)}" : "loaded neither module-element-icons nor module-svg-icon");
        yield return new Check("Icon applied", applied,
            applied ? "the Cacheable stereotype's icon is the approved SVG's data URI" : "the Cacheable stereotype has no icon from the approved SVG");
        yield return new Check("Through an included script", included > 0,
            included > 0 ? $"{included} run_designer_script call(s) passed includedScriptPaths" : "no run_designer_script call passed includedScriptPaths");
        yield return Icon.PayloadKeptOut(evidence, Icon.ApprovedSvg);
        yield return new Check("Config untouched", !evidence.Changed(Icon.Config),
            evidence.Changed(Icon.Config) ? "the application config changed - that is the package icon, not the stereotype's" : "the application config is unchanged");
        yield return new Check("Metadata by designer only", handEdited.Count == 0,
            handEdited.Count == 0 ? "designer metadata changed only through the designer" : $"hand-edited {string.Join(", ", handEdited)}");
        yield return new Check("No image left behind", images.Count == 0,
            images.Count == 0 ? "no .svg or .png left in the workspace" : $"left {string.Join(", ", images)}");
        yield return EndedCleanly(evidence);
    }

    private static bool IncludesScriptFiles(StubCall call)
    {
        try
        {
            using var arguments = System.Text.Json.JsonDocument.Parse(call.Arguments);
            return arguments.RootElement.TryGetProperty("includedScriptPaths", out var paths)
                && paths.ValueKind == System.Text.Json.JsonValueKind.Array && paths.GetArrayLength() > 0;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
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

    protected override string[][] AnswerMarkers => [["-100", "-10", "negative"]];

    protected override string Expectation => "register with a lower, negative priority";
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

    public override string Prompt =>
        "A new SampleModule template generates a `Handle` method whose body the developer writes by hand. The signature should " +
        "keep following the model, and the body must survive later Software Factory runs. How should the template emit that method?" + AnswerOnly;

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

/// <summary>C16 - Common.CSharp 3.10.12 needs Common 3.11.0, which needs SoftwareFactory.SDK 3.14.0.</summary>
public sealed class Nu1605ChainRoute : InstructionRoute
{
    public override string Id => "C16";

    public override string Title => "The NU1605 chain for 3.10.12";

    public override string Prompt =>
        "To use collection-item `Remove()` in SampleModule/Templates/Sample/SampleTemplatePartial.cs I raised " +
        "`Intent.Modules.Common.CSharp` to 3.10.12. Restore now fails with `NU1605: Detected package downgrade: " +
        "Intent.Modules.Common from 3.11.0 to 3.7.2`. I'll raise that one. What else will I need to change before it builds?" + AnswerOnly;

    protected override string[] Skills => ["known-build-gotchas", "file-builder-expert"];

    protected override string[][] AnswerMarkers => [["3.14.0"], ["softwarefactory.sdk"]];

    protected override string Expectation => "Intent.SoftwareFactory.SDK raised to 3.14.0";
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

/// <summary>C20 - TemplateRoles.Domain.Entity.Primary's value is "Domain.Entity".</summary>
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

/// <summary>C23 - another module is linked by its docs.intentarchitect.com page, with the slug derived from its id.</summary>
public sealed class ModuleDocsLinkRoute : RoutingScenario
{
    public override string Id => "C23";

    public override string Title => "Linking another module in docs";

    public override string Prompt =>
        "In SampleModule's docs/README.md, the Related Modules section should link to Intent.Eventing.NServiceBus. " +
        "What URL should the link use? Just tell me; don't change any files.";

    protected override string[] Skills => ["module-docs"];

    protected override string[][] AnswerMarkers => [["docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-nservicebus"]];

    protected override string Expectation => "the docs.intentarchitect.com page with the derived slug";
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
