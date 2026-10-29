namespace AgentEvals;

// ---------------------------------------------------------------------------------------------
// Layer 3 - guidance actually used: Intent.ModuleBuilder.AI.Guidelines routing
// ---------------------------------------------------------------------------------------------
//
// R1, R2, R4 and R5 covered skills that Intent Architect 5.4 ships built in, so they are no longer
// tested from here; their ids stay retired so old reports still read the same.

/// <summary>
/// A how-to question that only one of the AI.Guidelines skills answers well. It checks that the agent
/// finds that guidance and that its answer carries the rule the guidance teaches. A route is any
/// one of <see cref="Skills"/>, so the same scenario can be scored before and after a skill is
/// folded into another one.
/// </summary>
public abstract class RoutingScenario : Scenario
{
    public override Layer Layer => Layer.Guidance;

    public override bool UsesIntentStub => false;

    /// <summary>Skills or instructions that carry the answer. Loading any one of them counts.</summary>
    protected abstract string[] Skills { get; }

    /// <summary>
    /// Whether finding the guidance is a hard check. Not when an instruction carries the answer: a harness that
    /// loads it up front leaves no tool call to see, so the check is then only informational.
    /// </summary>
    protected virtual bool GuidanceLeavesATrace => true;

    /// <summary>What the answer must say: every group needs at least one of its words, matched lower-case.</summary>
    protected abstract string[][] AnswerMarkers { get; }

    protected abstract string Expectation { get; }

    protected const string AnswerOnly = " Give me the concrete steps, briefly; don't change any files.";

    public override IEnumerable<Check> Checks(Evidence evidence)
    {
        // Found = the guidance's own text reached the agent by any route, a grep of the skills folder included.
        // Loaded = it was loaded as a skill: routing, reported beside it but never failing the run.
        var found = Skills.Where(name => evidence.SkillContentReached(name) || evidence.FileReadAt($"{name}.instructions.md") >= 0).ToList();
        var loaded = evidence.SkillsLoaded(Skills);

        yield return new Check("Guidance found", found.Count > 0 || !GuidanceLeavesATrace,
            (found.Count > 0 ? $"reached {string.Join(", ", found)}" : $"reached none of {string.Join(", ", Skills)}")
            + (GuidanceLeavesATrace ? "" : " (informational: an instruction may be loaded up front)"));
        yield return new Check("Loaded as a skill", loaded.Count > 0,
            loaded.Count > 0 ? $"loaded {string.Join(", ", loaded)}" : "found by search or file read, or not at all", Informational: true);
        yield return Reported(evidence, Expectation, AnswerMarkers);
        yield return new Check("Nothing changed", evidence.ChangedFiles.Count == 0,
            evidence.ChangedFiles.Count == 0 ? "no file changed" : $"changed {string.Join(", ", evidence.ChangedFiles)}");
    }
}

/// <summary>
/// R3 - a bespoke stereotype icon is a UrlImagePath data URI, set in the Module Builder designer (by a
/// designer script, or by hand). The package icon's procedure also uses UrlImagePath and a data URI, so only
/// the second marker group leans towards the stereotype route, and loosely: an answer can name the designer
/// and still edit .application.config. Checking for that by keyword failed correct answers that mention the
/// package icon only to contrast it, so the judge, not a hard check, scores which route the answer took.
/// </summary>
public sealed class ElementIconRoute : RoutingScenario
{
    public override string Id => "R3";

    public override string Title => "Icon guidance for a stereotype";

    public override string Prompt =>
        "I want a stereotype I defined in SampleModule to show its own custom SVG icon in the designer, not a built-in one. " +
        "How do I set that up?" + AnswerOnly;

    protected override string[] Skills => ["module-element-icons", "module-svg-icon"];

    protected override string[][] AnswerMarkers =>
        [["urlimagepath", "data:image/svg"], ["run_designer_script", "includedscriptpaths", "designer script", "module builder designer"]];


    protected override string Expectation => "the icon is a UrlImagePath data URI set in the Module Builder designer";

    public override string JudgeBrief =>
        "The user asked how to give a stereotype defined in their module its own custom SVG icon, and asked for no file changes. " +
        "Good behaviour: the agent answers with the stereotype route - the stereotype definition's icon, in the Module Builder " +
        "designer, set to type UrlImagePath with a data:image/svg+xml;base64 source, the base64 computed by a script and applied " +
        "with run_designer_script (includedScriptPaths) rather than read or pasted by the agent. Bad behaviour: the package-icon " +
        "route instead - editing the root <icon>/<iconType> of the module's .application.config - which changes the module's own " +
        "icon, not the stereotype's. Mentioning the package icon only to contrast it is fine.";
}
