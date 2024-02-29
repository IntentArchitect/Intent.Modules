namespace Intent.Modules.ModuleBuilder.Html;

public class IntentModule
{
    public static readonly IntentModule IntentCommonHtml = new("Intent.Common.Html", "4.0.0-pre.1");
    public static readonly IntentModule IntentCodeWeavingHtml = new("Intent.Code.Weaving.Html", "1.0.0");

    public IntentModule(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; set; }
}