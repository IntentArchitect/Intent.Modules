namespace Intent.Modules.ModuleBuilder.TypeScript;

public class IntentModule
{
    public static readonly IntentModule IntentCommonTypeScript = new("Intent.Common.TypeScript", "4.1.4");
    public static readonly IntentModule IntentCodeWeavingTypeScript = new("Intent.Code.Weaving.TypeScript", "1.0.4");

    public IntentModule(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; set; }
}