namespace Intent.Modules.ModuleBuilder.CSharp;

public class IntentModule(string name, string version)
{
    public static readonly IntentModule IntentBlazor = new("Intent.Blazor", "1.0.0-alpha.0");
    public static readonly IntentModule IntentCommon = new("Intent.Common", "3.6.0");
    public static readonly IntentModule IntentCommonCSharp = new("Intent.Common.CSharp", "3.7.0-pre.1");

    public string Name { get; set; } = name;
    public string Version { get; set; } = version;
}