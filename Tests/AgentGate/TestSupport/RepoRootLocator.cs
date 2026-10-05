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
    /// The gate copy these tests run, taken from the TEST application rather than the repository root.
    /// </summary>
    /// <remarks>
    /// Each harness folder carries its own self-contained copy under "&lt;harness&gt;/hooks/gate", and
    /// ".agents" is the neutral one - always generated, owned by no single harness.
    /// <para>
    /// It comes from "Tests/ModuleBuilderSkills" because that is the application harness testing uses;
    /// the repository root's copy belongs to the dogfood app, which exists to be what an agent working
    /// in this repo runs on. Sourcing unit tests from one application and harness tests from another
    /// was a standing source of confusion, so both now come from the test app.
    /// </para>
    /// </remarks>
    public static string FindGateEntryPoint() =>
        Path.Combine(Find(), "Tests", "ModuleBuilderSkills", ".agents", "hooks", "gate", "gate.cs");
}
