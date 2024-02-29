namespace Intent.Modules.ModuleBuilder.Java;

public class IntentModule
{
    public static readonly IntentModule IntentCommonJava = new("Intent.Common.Java", "4.0.0-pre.1");
    public static readonly IntentModule IntentCodeWeavingJava = new("Intent.Code.Weaving.Java", "1.0.0");

    public IntentModule(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; set; }
}