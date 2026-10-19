using System.Diagnostics;

namespace AgentEvals;

public sealed record ProcessResult(int ExitCode, string Stdout, string Stderr, bool TimedOut, TimeSpan Duration);

/// <summary>Starts a process with every redirected stream drained, and a hard timeout.</summary>
public static class Processes
{
    public static async Task<ProcessResult> RunAsync(ProcessStartInfo startInfo, string? stdin, TimeSpan timeout)
    {
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;

        var started = Stopwatch.StartNew();
        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException($"Could not start '{startInfo.FileName}'.");

        // Closed even when there is nothing to send: several CLIs wait for stdin to end before starting.
        if (stdin is not null)
        {
            await process.StandardInput.WriteAsync(stdin);
        }

        process.StandardInput.Close();

        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();

        using var cancellation = new CancellationTokenSource(timeout);
        var timedOut = false;
        try
        {
            await process.WaitForExitAsync(cancellation.Token);
        }
        catch (OperationCanceledException)
        {
            timedOut = true;
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
                // Already gone.
            }

            await process.WaitForExitAsync();
        }

        return new ProcessResult(timedOut ? -1 : process.ExitCode, await stdout, await stderr, timedOut, started.Elapsed);
    }

    public static ProcessStartInfo Command(string fileName, string workingDirectory, params string[] args)
    {
        var (file, prefix) = OperatingSystem.IsWindows() && fileName.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase)
            ? Unshim(fileName)
            : (fileName, []);
        var startInfo = new ProcessStartInfo(file) { WorkingDirectory = workingDirectory };
        foreach (var arg in prefix.Concat(args))
        {
            startInfo.ArgumentList.Add(arg);
        }

        // A shell such as Git Bash exports PWD, which a child inherits unchanged - and some CLIs (OpenCode)
        // take their project folder from PWD rather than the real working directory. Left alone, that sends
        // an agent to work in whatever folder the runner was started from: the repository itself.
        startInfo.Environment["PWD"] = workingDirectory;
        startInfo.Environment.Remove("OLDPWD");
        return startInfo;
    }

    /// <summary>
    /// An npm ".cmd" shim runs through cmd.exe, which re-parses its arguments: a prompt containing
    /// &lt; &gt; | or &amp; would be read as redirection. The shim names what it really launches - an .exe,
    /// or a .js run by node - so that is started directly instead.
    /// </summary>
    private static (string File, string[] Prefix) Unshim(string shim)
    {
        var folder = Path.GetDirectoryName(shim)!;
        // e.g.  "%dp0%\node_modules\opencode-ai\bin\opencode.exe"   %*
        //  or   "%_prog%"  "%dp0%\node_modules\@github\copilot\npm-loader.js" %*   (after probing for a bundled node.exe)
        var targets = System.Text.RegularExpressions.Regex.Matches(File.ReadAllText(shim), @"""%dp0%\\([^""]+\.(exe|js))""")
            .Select(m => (Path: Path.Combine(folder, m.Groups[1].Value), Kind: m.Groups[2].Value.ToLowerInvariant()))
            .ToList();
        if (targets.FirstOrDefault(t => t.Kind == "js") is { Path: not null } script)
        {
            var bundled = Path.Combine(folder, "node.exe");
            return (File.Exists(bundled) ? bundled : Find("node") ?? "node", [script.Path]);
        }

        return targets.FirstOrDefault(t => t.Kind == "exe") is { Path: not null } exe ? (exe.Path, []) : (shim, []);
    }

    /// <summary>
    /// Resolves a CLI on PATH. On Windows several are ".cmd" shims, which ProcessStartInfo cannot start
    /// by bare name, so the shim's full path is returned instead.
    /// </summary>
    public static string? Find(string name)
    {
        var extensions = OperatingSystem.IsWindows() ? new[] { ".exe", ".cmd", ".bat", "" } : new[] { "" };
        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
        {
            foreach (var extension in extensions)
            {
                var candidate = Path.Combine(directory, name + extension);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return null;
    }
}
