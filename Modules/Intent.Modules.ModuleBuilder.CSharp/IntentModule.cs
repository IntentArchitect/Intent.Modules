namespace Intent.Modules.ModuleBuilder.CSharp;

public class IntentModule
{
    public static readonly IntentModule IntentCommonCSharp = new("Intent.Common.CSharp", "3.7.3-pre.0");

    public IntentModule(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; set; }
}