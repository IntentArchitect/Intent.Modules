namespace Intent.Modules.Common.TypeScript.Templates;

public class NpmPackageDependency
{
    public NpmPackageDependency(string name, string version, bool isDevDependency = false)
    {
        Name = name;
        Version = version;
        IsDevDependency = isDevDependency;
    }

    public string Name { get; set; }
    public string Version { get; set; }
    public bool IsDevDependency { get; set; }
}