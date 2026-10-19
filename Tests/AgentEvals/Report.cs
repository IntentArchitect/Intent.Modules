using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace AgentEvals;

/// <summary>The outcome of one harness running one scenario once - or of one harness's wiring checks.</summary>
public sealed record RunResult(
    string Harness,
    string HarnessVersion,
    string Scenario,
    string ScenarioTitle,
    IReadOnlyList<Check> Checks,
    IReadOnlyList<JudgeScore> JudgeScores,
    int Turns,
    int GateRuns,
    double Seconds,
    string? Error,
    string Layer = "Wiring",
    int Attempt = 1)
{
    /// <summary>Hard checks count 2 when passed and 0 when failed; judge scores count as given.</summary>
    public int Points => Checks.Sum(c => c.Passed ? 2 : 0) + JudgeScores.Sum(s => s.Score);

    public int MaxPoints => (Checks.Count + JudgeScores.Count) * 2;

    /// <summary>Every hard check passed. Judge scores grade quality; they never decide a pass.</summary>
    public bool Passed => Error is null && Checks.Count > 0 && Checks.All(c => c.Passed);

    public string Rating => Error is not null ? "error" : MaxPoints == 0 ? "-" : $"{Points}/{MaxPoints}";
}

public static class Report
{
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };

    public static string Write(string runDirectory, RunInfo info, IReadOnlyList<RunResult> results)
    {
        Directory.CreateDirectory(runDirectory);
        File.WriteAllText(Path.Combine(runDirectory, "summary.json"), JsonSerializer.Serialize(new { info, results }, Json));

        var md = new StringBuilder();
        md.AppendLine($"# Agent evals - {info.StartedAt:yyyy-MM-dd HH:mm}");
        md.AppendLine();
        md.AppendLine($"- OS: {RuntimeInformation.OSDescription}");
        md.AppendLine($"- Inputs: commit {info.Commit}{(info.Dirty ? " with uncommitted changes" : "")}");
        foreach (var (harness, version) in info.HarnessVersions)
        {
            md.AppendLine($"- {harness}: {version}");
        }

        foreach (var (harness, reason) in info.Skipped)
        {
            md.AppendLine($"- {harness}: skipped - {reason}");
        }

        var wiring = results.Where(r => r.Layer == "Wiring").ToList();
        if (wiring.Count > 0)
        {
            md.AppendLine();
            md.AppendLine("## Layer 1 - wiring");
            md.AppendLine();
            var dimensions = wiring.SelectMany(r => r.Checks.Select(c => c.Dimension)).Distinct().ToList();
            md.AppendLine("| Harness | " + string.Join(" | ", dimensions) + " |");
            md.AppendLine("|---|" + string.Concat(dimensions.Select(_ => "---|")));
            foreach (var result in wiring)
            {
                md.AppendLine($"| {result.Harness} | " + string.Join(" | ", dimensions.Select(d =>
                    result.Checks.FirstOrDefault(c => c.Dimension == d) is { } c ? (c.Passed ? "pass" : "**FAIL**") : "-")) + " |");
            }

            md.AppendLine();
            foreach (var (result, check) in wiring.SelectMany(r => r.Checks.Where(c => !c.Passed).Select(c => (r, c))))
            {
                md.AppendLine($"- **{result.Harness} - {check.Dimension}:** {check.Detail}");
            }
        }

        foreach (var layer in new[] { "Hooks", "Guidance", "AdHoc" })
        {
            var runs = results.Where(r => r.Layer == layer).ToList();
            if (runs.Count == 0)
            {
                continue;
            }

            var scenarios = runs.Select(r => (r.Scenario, r.ScenarioTitle)).Distinct().ToList();
            md.AppendLine();
            md.AppendLine($"## {(layer == "Hooks" ? "Layer 2 - hooks" : layer == "Guidance" ? "Layer 3 - guidance used" : "Ad-hoc prompt")}");
            md.AppendLine();
            md.AppendLine("| Harness | " + string.Join(" | ", scenarios.Select(s => $"{s.Scenario} {s.ScenarioTitle}")) + " |");
            md.AppendLine("|---|" + string.Concat(scenarios.Select(_ => "---|")));
            foreach (var harness in runs.GroupBy(r => r.Harness))
            {
                md.AppendLine($"| {harness.Key} | " + string.Join(" | ", scenarios.Select(s => Cell(harness.Where(r => r.Scenario == s.Scenario).ToList()))) + " |");
            }
        }

        foreach (var result in results.Where(r => r.Layer != "Wiring"))
        {
            md.AppendLine();
            var attempt = results.Count(r => r.Harness == result.Harness && r.Scenario == result.Scenario) > 1 ? $" (attempt {result.Attempt})" : "";
            md.AppendLine($"### {result.Harness} - {result.Scenario} {result.ScenarioTitle}{attempt}: {(result.Passed ? "pass" : "FAIL")} {result.Rating}");
            md.AppendLine();
            if (result.Error is not null)
            {
                md.AppendLine($"Run failed: {result.Error}");
                continue;
            }

            md.AppendLine($"{result.Turns} turn(s), {result.GateRuns} gate run(s), {result.Seconds:0}s");
            md.AppendLine();
            md.AppendLine("| Dimension | Result | Detail |");
            md.AppendLine("|---|---|---|");
            foreach (var check in result.Checks)
            {
                md.AppendLine($"| {check.Dimension} | {(check.Passed ? "pass" : "**FAIL**")} | {Escape(check.Detail)} |");
            }

            foreach (var score in result.JudgeScores)
            {
                md.AppendLine($"| {score.Dimension} (judge) | {score.Score}/2 | {Escape(score.Evidence)} |");
            }
        }

        var summaryPath = Path.Combine(runDirectory, "summary.md");
        File.WriteAllText(summaryPath, md.ToString());
        return summaryPath;
    }

    /// <summary>One run: pass/FAIL and points. Repeated runs: the pass rate.</summary>
    private static string Cell(IReadOnlyList<RunResult> runs) => runs.Count switch
    {
        0 => "-",
        1 => runs[0].Error is not null ? "error" : $"{(runs[0].Passed ? "pass" : "**FAIL**")} {runs[0].Rating}",
        _ => $"{runs.Count(r => r.Passed)}/{runs.Count} pass",
    };

    private static string Escape(string value) => value.ReplaceLineEndings(" ").Replace("|", "\\|");
}

/// <summary>What was run, against which inputs - written to run.json and the top of the report.</summary>
public sealed record RunInfo(
    DateTime StartedAt,
    string Commit,
    bool Dirty,
    IReadOnlyList<string> Arguments,
    Dictionary<string, string> HarnessVersions,
    Dictionary<string, string> Skipped);
