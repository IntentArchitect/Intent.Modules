namespace Intent.Modules.ModuleBuilder
{
    public class IntentModule
    {
        public static readonly IntentModule IntentRoslynWeaver = new("Intent.OutputManager.RoslynWeaver", "3.3.8");
        public static readonly IntentModule IntentCommon = new("Intent.Common", "3.3.16-pre.0");
        public static readonly IntentModule IntentCommonTypes = new("Intent.Common.Types", "3.3.6");

        public IntentModule(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; set; }
        public string Version { get; set; }
    }
}
