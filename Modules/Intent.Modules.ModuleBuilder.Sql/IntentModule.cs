namespace Intent.Modules.ModuleBuilder.Sql;

public class IntentModule
{
    public static readonly IntentModule IntentCommonSql = new("Intent.Common.Sql", "3.4.0");

    public IntentModule(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; set; }
}