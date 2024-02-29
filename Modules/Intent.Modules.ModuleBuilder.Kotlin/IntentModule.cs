namespace Intent.Modules.ModuleBuilder.Kotlin;

public class IntentModule
{
    public static readonly IntentModule IntentCommonKotlin = new("Intent.Common.Kotlin", "4.0.0-pre.0");
    public static readonly IntentModule IntentCodeWeavingKotlin = new("Intent.Code.Weaving.Kotlin", "1.0.0");

    public IntentModule(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; set; }
}