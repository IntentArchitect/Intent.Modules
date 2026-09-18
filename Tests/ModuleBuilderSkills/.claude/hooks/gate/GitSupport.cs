using System.Diagnostics;
using System.Text.Json;

namespace Intent.Agent.Gate;

/// <summary>
/// Finds the root of the git repository (or worktree) containing a given directory, by
/// walking upward looking for a ".git" entry. In a worktree, ".git" is a file (pointing at
/// the main repository's worktree metadata) rather than a directory, so both are treated as
/// a match.
/// </summary>
public static class GitRepoLocator
{
    public static string? FindRepoRoot(string startDirectory)
    {
        var current = new DirectoryInfo(startDirectory).FullName;

        while (true)
        {
            var dotGit = Path.Combine(current, ".git");
            if (Directory.Exists(dotGit) || File.Exists(dotGit))
            {
                return current;
            }

            var parent = Directory.GetParent(current);
            if (parent is null)
            {
                return null;
            }

            current = parent.FullName;
        }
    }
}

/// <summary>
/// Abstraction over "what files changed" and "what a file looked like at HEAD", so gate
/// logic can be unit-tested against a fake without shelling out to git.
/// </summary>
public interface IGitChangeProvider
{
    /// <summary>
    /// Paths (relative to <paramref name="repoRoot"/>, forward-slash separated, matching
    /// git's own output convention) of files that differ between the working tree and HEAD.
    /// </summary>
    IReadOnlyList<string> GetChangedFiles(string repoRoot);

    /// <summary>
    /// The content of <paramref name="relativePath"/> as it was at HEAD, or null if the path
    /// did not exist at HEAD (e.g. a newly-added file, or a module newly created in this
    /// line of work).
    /// </summary>
    string? TryReadFileAtHead(string repoRoot, string relativePath);
}

/// <summary>
/// Real <see cref="IGitChangeProvider"/> that shells out to git.
/// </summary>
public sealed class ProcessGitChangeProvider : IGitChangeProvider
{
    public IReadOnlyList<string> GetChangedFiles(string repoRoot)
    {
        // "git diff" alone misses brand-new (untracked) files entirely - a module
        // created from scratch in this line of work would otherwise be invisible to
        // the audit below. Union tracked changes against HEAD with untracked files.
        var changed = RunGit(repoRoot, "diff", "--name-only", "HEAD");
        var untracked = RunGit(repoRoot, "ls-files", "--others", "--exclude-standard");

        return SplitLines(changed).Concat(SplitLines(untracked)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static IEnumerable<string> SplitLines(string? output) =>
        output is null
            ? []
            : output.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0);

    public string? TryReadFileAtHead(string repoRoot, string relativePath)
    {
        return RunGit(repoRoot, "show", $"HEAD:{relativePath}");
    }

    private static string? RunGit(string repoRoot, params string[] args)
    {
        var startInfo = new ProcessStartInfo("git")
        {
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            return null;
        }

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        // A non-zero exit (e.g. "show HEAD:path" for a path that did not exist at HEAD)
        // means no content to read, not an error worth surfacing to the caller.
        return process.ExitCode == 0 ? output : null;
    }
}

/// <summary>
/// Extracts a target file path from a PreToolUse-style hook payload on stdin. The exact
/// JSON shape differs per harness (Claude Code / Codex nest it under "tool_input"; Cursor's
/// and Kiro's exact shapes are not confirmed from documentation), so this does a tolerant
/// search: try a short list of candidate key names at the top level, then one level of
/// nesting inside any object property. A positional CLI argument is accepted as a fallback,
/// for manual testing or a harness whose payload this doesn't recognise.
/// </summary>
public static class StdinPathExtractor
{
    private static readonly string[] CandidateKeys = ["file_path", "filePath", "path"];

    public static string? Extract(string input, string? positionalArg = null)
    {
        var fromJson = TryExtractFromJson(input);
        if (!string.IsNullOrWhiteSpace(fromJson))
        {
            return fromJson;
        }

        return string.IsNullOrWhiteSpace(positionalArg) ? null : positionalArg;
    }

    private static string? TryExtractFromJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(input);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            foreach (var key in CandidateKeys)
            {
                if (TryGetString(root, key, out var value))
                {
                    return value;
                }
            }

            foreach (var property in root.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                foreach (var key in CandidateKeys)
                {
                    if (TryGetString(property.Value, key, out var value))
                    {
                        return value;
                    }
                }
            }

            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool TryGetString(JsonElement element, string propertyName, out string value)
    {
        value = string.Empty;

        if (element.ValueKind == JsonValueKind.Object &&
            element.TryGetProperty(propertyName, out var property) &&
            property.ValueKind == JsonValueKind.String)
        {
            var raw = property.GetString();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                value = raw;
                return true;
            }
        }

        return false;
    }
}