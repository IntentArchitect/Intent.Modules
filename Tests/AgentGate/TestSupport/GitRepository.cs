using System.Diagnostics;

namespace AgentGate.Tests.TestSupport;

/// <summary>
/// A real git repository in a temp directory, for the behaviour only git itself can answer - what is
/// unmerged, untracked or ignored - where <see cref="FakeGitChangeProvider"/> would prove nothing.
/// </summary>
public sealed class GitRepository : IDisposable
{
    private readonly TempDirectory _directory = new();

    private GitRepository()
    {
    }

    public string Path => _directory.Path;

    public static GitRepository Init()
    {
        var repo = new GitRepository();
        repo.Git("init", "-q", "-b", "main");
        repo.Git("config", "user.email", "gate@test");
        repo.Git("config", "user.name", "gate");
        repo.Git("config", "core.autocrlf", "false");
        return repo;
    }

    public string FullPath(string relativePath) => System.IO.Path.Combine(Path, relativePath);

    public string CreateFile(string relativePath, string content) => _directory.CreateFile(relativePath, content);

    public void Commit(string relativePath, string content, string message)
    {
        CreateFile(relativePath, content);
        Git("add", "-A");
        Git("commit", "-q", "-m", message);
    }

    public string Git(params string[] args) => Git(allowFailure: false, args);

    public string Git(bool allowFailure, params string[] args)
    {
        var startInfo = new ProcessStartInfo("git")
        {
            WorkingDirectory = Path,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = Process.Start(startInfo)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        if (!allowFailure && process.ExitCode != 0)
        {
            throw new InvalidOperationException($"git {string.Join(' ', args)} failed: {stderr.Result}");
        }

        return stdout.Result;
    }

    public void Dispose()
    {
        // git writes its objects read-only, which a recursive delete on Windows refuses.
        foreach (var file in Directory.EnumerateFiles(Path, "*", SearchOption.AllDirectories))
        {
            File.SetAttributes(file, FileAttributes.Normal);
        }

        _directory.Dispose();
    }
}
