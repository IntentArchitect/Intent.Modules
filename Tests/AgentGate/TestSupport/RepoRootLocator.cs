namespace AgentGate.Tests.TestSupport;

/// <summary>
/// Finds this repository's root from the test assembly's own build output directory, by walking
/// up looking for ".git" - deliberately not a fixed "../../.." count, which breaks the moment the
/// build configuration or target framework folder depth changes.
/// </summary>
public static class RepoRootLocator
{
    public static string Find()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, ".git")) || File.Exists(Path.Combine(current.FullName, ".git")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException($"Could not locate the repository root by walking up from '{AppContext.BaseDirectory}'.");
    }

    /// <summary>
    /// Each harness folder carries its own self-contained copy under "&lt;harness&gt;/hooks/gate". The
    /// tests deliberately use the ".agents" one: it is the neutral folder, always generated, and not
    /// owned by any single harness.
    /// </summary>
    public static string FindGateEntryPoint() => Path.Combine(Find(), ".agents", "hooks", "gate", "gate.cs");
}
