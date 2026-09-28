using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

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
        var changed = GitProcess.Run(repoRoot, "diff", "--name-only", "HEAD");
        var untracked = GitProcess.Run(repoRoot, "ls-files", "--others", "--exclude-standard");

        return GitProcess.SplitLines(changed).Concat(GitProcess.SplitLines(untracked))
            .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public string? TryReadFileAtHead(string repoRoot, string relativePath)
    {
        return GitProcess.Run(repoRoot, "show", $"HEAD:{relativePath}");
    }
}

/// <summary>
/// The single place this gate shells out to git. Every caller gets the same
/// fail-soft behaviour: a missing "git" executable, or any other failure to start the
/// process, returns null rather than throwing - callers that can fall back to a
/// non-git mechanism (see <see cref="ModuleDiscovery"/>) get the chance to.
/// </summary>
internal static class GitProcess
{
    public static string? Run(string repoRoot, params string[] args)
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

        try
        {
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // A non-zero exit (e.g. "show HEAD:path" for a path that did not exist at
            // HEAD, or git not being a recognised command at all) means no content to
            // read, not an error worth surfacing to the caller.
            return process.ExitCode == 0 ? output : null;
        }
        catch (Exception)
        {
            // git is not installed, not on PATH, or could not be started for some other
            // reason. Fail soft - the caller decides whether that is fatal or has a
            // fallback.
            return null;
        }
    }

    public static IEnumerable<string> SplitLines(string? output) =>
        output is null
            ? []
            : output.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0);
}

/// <summary>
/// One entry per Intent Architect application discovered in the repo: its id (from
/// ".application.config"), and its real OUTPUT root - resolved from that config's own
/// "location" attribute, which is not always the folder the config itself sits in (a
/// solution whose workspace root differs from its metadata folder splits the two).
/// </summary>
public sealed record DiscoveredApplication(string ApplicationId, string ConfigPath, string OutputRoot);

/// <summary>
/// Finds every Intent Architect application in the repo without assuming any particular
/// folder layout - no hardcoded "Modules/" or similar. A repo laid out differently (a
/// module author outside this monorepo, or a split metadata/output-root solution like
/// one built from the Application Template Builder) is still found correctly.
/// </summary>
public static class ModuleDiscovery
{
    private static readonly string[] SkippedDirectoryNames =
    {
        ".git", "bin", "obj", "node_modules", ".vs", ".idea", ".cache",
    };

    public static IReadOnlyList<DiscoveredApplication> FindAll(string repoRoot)
    {
        var discovered = new List<DiscoveredApplication>();

        foreach (var configPath in EnumerateApplicationConfigs(repoRoot))
        {
            try
            {
                var document = XDocument.Load(configPath);
                var id = (string?)document.Root?.Attribute("id");
                if (id is null)
                {
                    continue;
                }

                var location = (string?)document.Root?.Attribute("location") ?? ".";
                var configDir = Path.GetDirectoryName(configPath)!;
                var outputRoot = Path.GetFullPath(Path.Combine(configDir, location));

                discovered.Add(new DiscoveredApplication(id, configPath, outputRoot));
            }
            catch (Exception)
            {
                // A malformed application.config should not crash discovery - keep looking.
            }
        }

        return discovered;
    }

    /// <summary>
    /// Finds the ".imodspec" belonging to an application's output root. Deliberately
    /// bounded to the root itself and its immediate subfolders (a module's own project
    /// can sit one folder deeper, e.g. "NewModule/NewModule/NewModule.imodspec") rather
    /// than an unbounded recursive search - an application whose output root is an
    /// ancestor of many unrelated modules (e.g. this repo's own dogfood app, whose
    /// output root is the repository root) must correctly report "not a module" rather
    /// than picking up an arbitrary descendant's imodspec.
    /// </summary>
    public static string? FindImodspecUnder(string outputRoot)
    {
        if (!Directory.Exists(outputRoot))
        {
            return null;
        }

        var atRoot = Directory.EnumerateFiles(outputRoot, "*.imodspec", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (atRoot is not null)
        {
            return atRoot;
        }

        foreach (var subDirectory in Directory.EnumerateDirectories(outputRoot))
        {
            var name = Path.GetFileName(subDirectory);
            if (Array.Exists(SkippedDirectoryNames, skip => string.Equals(skip, name, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var match = Directory.EnumerateFiles(subDirectory, "*.imodspec", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    /// <summary>
    /// Attributes each changed file (repo-root-relative, forward-slash) to the discovered
    /// application whose output root contains it, by longest-prefix match - so a file
    /// under a nested module's own output root is attributed there, not to an ancestor
    /// application (e.g. a shared dogfood app) whose root merely happens to contain it
    /// too.
    /// </summary>
    public static IReadOnlyDictionary<DiscoveredApplication, IReadOnlyList<string>> GroupChangedFilesByApplication(
        string repoRoot, IReadOnlyList<string> normalizedChangedFiles)
    {
        var applications = FindAll(repoRoot);
        var grouped = new Dictionary<DiscoveredApplication, List<string>>();

        foreach (var file in normalizedChangedFiles)
        {
            var absolute = Path.GetFullPath(Path.Combine(repoRoot, file));

            DiscoveredApplication? best = null;
            var bestLength = -1;
            foreach (var application in applications)
            {
                var root = application.OutputRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var isUnderRoot = absolute.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(absolute, root, StringComparison.OrdinalIgnoreCase);

                if (isUnderRoot && root.Length > bestLength)
                {
                    best = application;
                    bestLength = root.Length;
                }
            }

            if (best is null)
            {
                continue;
            }

            if (!grouped.TryGetValue(best, out var list))
            {
                list = new List<string>();
                grouped[best] = list;
            }

            list.Add(file);
        }

        return grouped.ToDictionary(pair => pair.Key, IReadOnlyList<string> (pair) => pair.Value);
    }

    /// <summary>
    /// The fast, precise path: ask git which ".application.config" files exist, tracked
    /// or not (a module created moments ago and never "git add"-ed must still be found).
    /// Falls back to a bounded filesystem walk only when git itself could not be asked at
    /// all (not on PATH, or this is not a git repository) - never when git ran fine and
    /// simply found nothing.
    /// </summary>
    private static IEnumerable<string> EnumerateApplicationConfigs(string repoRoot)
    {
        var tracked = GitProcess.Run(repoRoot, "ls-files", "--", "*.application.config");
        var untracked = GitProcess.Run(repoRoot, "ls-files", "--others", "--exclude-standard", "--", "*.application.config");

        if (tracked is null && untracked is null)
        {
            return EnumerateViaFilesystem(repoRoot);
        }

        return GitProcess.SplitLines(tracked).Concat(GitProcess.SplitLines(untracked))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(relative => Path.GetFullPath(Path.Combine(repoRoot, relative.Replace('/', Path.DirectorySeparatorChar))));
    }

    private static IEnumerable<string> EnumerateViaFilesystem(string repoRoot)
    {
        var results = new List<string>();
        WalkDirectory(repoRoot, results);
        return results;
    }

    private static void WalkDirectory(string directory, List<string> results)
    {
        IEnumerable<string> files;
        IEnumerable<string> subDirectories;
        try
        {
            files = Directory.EnumerateFiles(directory, "*.application.config", SearchOption.TopDirectoryOnly);
            subDirectories = Directory.EnumerateDirectories(directory).ToList();
        }
        catch (Exception)
        {
            return;
        }

        results.AddRange(files);

        foreach (var subDirectory in subDirectories)
        {
            var name = Path.GetFileName(subDirectory);
            if (Array.Exists(SkippedDirectoryNames, skip => string.Equals(skip, name, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            WalkDirectory(subDirectory, results);
        }
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