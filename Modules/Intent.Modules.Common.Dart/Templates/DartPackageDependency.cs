namespace Intent.Modules.Common.Dart.Templates;

/// <summary>
/// A Dart package dependency.
/// </summary>
public class DartPackageDependency
{
    /// <summary>
    /// Creates a new instance of <see cref="DartPackageDependency"/>.
    /// </summary>
    public DartPackageDependency(string name, string version, bool isDevDependency = false)
    {
        Name = name;
        Version = version;
        IsDevDependency = isDevDependency;
    }

    /// <summary>
    /// The name of the package.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The version of the package.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Whether or not this is a <see href="https://dart.dev/tools/pub/dependencies#dev-dependencies">dev dependency</see>.
    /// </summary>
    public bool IsDevDependency { get; set; }
}