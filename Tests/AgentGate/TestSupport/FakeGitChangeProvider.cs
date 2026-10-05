using Intent.Agent.Gate;

namespace AgentGate.Tests.TestSupport;

/// <summary>
/// Fake <see cref="IGitChangeProvider"/> so guard-version's double-bump guard and close-out's
/// change detection can be tested without shelling out to git or maintaining fixture repos.
/// </summary>
public sealed class FakeGitChangeProvider : IGitChangeProvider
{
    private readonly List<string> _changedFiles = [];
    private readonly Dictionary<string, string> _fileContentsAtHead = new(StringComparer.OrdinalIgnoreCase);

    public FakeGitChangeProvider WithChangedFiles(params string[] relativePaths)
    {
        _changedFiles.AddRange(relativePaths);
        return this;
    }

    public FakeGitChangeProvider WithFileAtHead(string relativePath, string content)
    {
        _fileContentsAtHead[relativePath] = content;
        return this;
    }

    public IReadOnlyList<string> GetChangedFiles(string repoRoot) => _changedFiles;

    public string? TryReadFileAtHead(string repoRoot, string relativePath) =>
        _fileContentsAtHead.TryGetValue(relativePath, out var content) ? content : null;
}
