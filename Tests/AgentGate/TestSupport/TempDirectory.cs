namespace AgentGate.Tests.TestSupport;

/// <summary>
/// A real, disposable temp directory on disk - used wherever a test needs actual files at actual
/// paths (managed-files.xml scanning, .imodspec reads), rather than mocking the filesystem.
/// </summary>
public sealed class TempDirectory : IDisposable
{
    public string Path { get; }

    public TempDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "agent-gate-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
    }

    public string CreateFile(string relativePath, string content)
    {
        var fullPath = System.IO.Path.Combine(Path, relativePath);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, content);
        return fullPath;
    }

    /// <summary>
    /// Marks this temp directory as a git repo root for <c>GitRepoLocator</c> - just a ".git" entry
    /// to walk up to, not a real repository (no working "git" commands are needed by these tests,
    /// since git-dependent behaviour goes through <see cref="FakeGitChangeProvider"/> instead).
    /// </summary>
    public void MarkAsRepoRoot() => Directory.CreateDirectory(System.IO.Path.Combine(Path, ".git"));

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
        catch (IOException)
        {
            // Best-effort cleanup - a locked file left behind by a fast-following test run
            // should not fail the suite.
        }
    }
}
