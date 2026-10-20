namespace AgentEvals;

// ---------------------------------------------------------------------------------------------
// Layer 3 - guidance actually used: Intent.ModuleBuilder.AI.Skills routing
// ---------------------------------------------------------------------------------------------

/// <summary>
/// A how-to question that only one of the AI.Skills skills answers well. It checks that the agent
/// finds that guidance and that its answer carries the rule the guidance teaches. A route is any
/// one of <see cref="Skills"/>, so the same scenario can be scored before and after a skill is
/// folded into another one.
/// </summary>
public abstract class RoutingScenario : Scenario
{
    public override Layer Layer => Layer.Guidance;

    public override bool UsesIntentStub => false;

    /// <summary>Skills that carry the answer. Loading any one of them counts.</summary>
    protected abstract string[] Skills { get; }

    /// <summary>What the answer must say: every group needs at least one of its words, matched lower-case.</summary>
    protected abstract string[][] AnswerMarkers { get; }

    protected abstract string Expectation { get; }

    protected const string AnswerOnly = " Give me the concrete steps, briefly; don't change any files.";

    public override IEnumerable<Check> Checks(Evidence evidence)
    {
        var loaded = evidence.SkillsLoaded(Skills);
        var answer = evidence.Transcript.FinalMessage.ToLowerInvariant();
        var missing = AnswerMarkers.Where(group => !group.Any(answer.Contains)).ToList();

        yield return new Check("Guidance found", loaded.Count > 0,
            loaded.Count > 0 ? $"loaded {string.Join(", ", loaded)}" : $"loaded none of {string.Join(", ", Skills)}");
        yield return new Check("Answer", missing.Count == 0,
            missing.Count == 0 ? $"the answer says {Expectation}"
            : $"the answer does not say {Expectation}: nothing about {string.Join(" / ", missing.Select(g => string.Join("|", g)))}");
        yield return new Check("Nothing changed", evidence.ChangedFiles.Count == 0,
            evidence.ChangedFiles.Count == 0 ? "no file changed" : $"changed {string.Join(", ", evidence.ChangedFiles)}");
    }
}

/// <summary>R1 - a custom mapping resolver hands the nodes it does not handle on with next?.Invoke.</summary>
public sealed class MappingResolverRoute : RoutingScenario
{
    public override string Id => "R1";

    public override string Title => "Mapping guidance for a custom resolver";

    public override string Prompt =>
        "SampleModule needs a C# template that turns a designer-modelled mapping (a command mapped onto an entity) into " +
        "assignment statements, with a custom resolver for one mapping type. Sketch how that resolver should be written." + AnswerOnly;

    protected override string[] Skills => ["intent-mapping-architect", "file-builder-expert"];

    protected override string[][] AnswerMarkers => [["next?.invoke"]];

    protected override string Expectation => "unhandled nodes go on through next?.Invoke";
}

/// <summary>R2 - a new kind of association is an Association Settings element with a source and a target end.</summary>
public sealed class AssociationTypeRoute : RoutingScenario
{
    public override string Id => "R2";

    public override string Title => "Designer guidance for a new association type";

    public override string Prompt =>
        "SampleModule needs a new kind of association that users can draw between two element types in the Domain designer. " +
        "What do I set up in the Module Builder designer?" + AnswerOnly;

    protected override string[] Skills => ["add-association-type", "add-designer-extension"];

    protected override string[][] AnswerMarkers => [["association settings"], ["target end", "target-end"]];

    protected override string Expectation => "it is an Association Settings element configured at its target end";
}

/// <summary>R3 - a bespoke stereotype icon is a UrlImagePath data URI, computed by a script.</summary>
public sealed class ElementIconRoute : RoutingScenario
{
    public override string Id => "R3";

    public override string Title => "Icon guidance for a stereotype";

    public override string Prompt =>
        "I want a stereotype I defined in SampleModule to show its own custom SVG icon in the designer, not a built-in one. " +
        "How do I set that up?" + AnswerOnly;

    protected override string[] Skills => ["module-element-icons", "module-svg-icon"];

    protected override string[][] AnswerMarkers => [["urlimagepath", "data:image/svg"]];

    protected override string Expectation => "the icon is a UrlImagePath data URI";
}

/// <summary>R4 - module code is debugged by logging through Logging.Log and reading the Software Factory log.</summary>
public sealed class ModuleDebuggingRoute : RoutingScenario
{
    public override string Id => "R4";

    public override string Title => "Debugging guidance for a template";

    public override string Prompt =>
        "SampleModule's template generates the wrong output, and I can't tell what value a model property has while the " +
        "Software Factory runs it. How do I see that value?" + AnswerOnly;

    protected override string[] Skills => ["module-debugging", "file-builder-expert"];

    protected override string[][] AnswerMarkers => [["logging.log"], ["software-factory"]];

    protected override string Expectation => "to log through Logging.Log and read the software-factory log";
}

/// <summary>R5 - control: a skill this change leaves alone. A drop here is noise, not the change.</summary>
public sealed class MetadataConsumerRoute : RoutingScenario
{
    public override string Id => "R5";

    public override string Title => "Metadata guidance for a stereotype option (control)";

    public override string Prompt =>
        "In a SampleModule template I need to branch on the 'Mode' option of a 'My Settings' stereotype applied to a class. " +
        "How should the template read that value?" + AnswerOnly;

    protected override string[] Skills => ["intent-metadata-consumer"];

    protected override string[][] AnswerMarkers => [["asenum", "optionsenum"]];

    protected override string Expectation => "to read the option through its typed enum helper";
}
