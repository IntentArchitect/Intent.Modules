namespace Intent.Modules.ModuleBuilder.Dart;

public class IntentModule
{
    public static readonly IntentModule IntentCommonDart = new("Intent.Common.Dart", "1.1.0");

    public IntentModule(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; set; }
}