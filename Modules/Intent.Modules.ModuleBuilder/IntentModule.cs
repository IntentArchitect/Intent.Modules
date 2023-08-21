namespace Intent.Modules.ModuleBuilder
{
    public class IntentModule
    {
        public static readonly IntentModule IntentRoslynWeaver = new("Intent.OutputManager.RoslynWeaver", "4.1.1");
        public static readonly IntentModule IntentCommon = new("Intent.Common", "3.4.3");
        public static readonly IntentModule IntentCommonTypes = new("Intent.Common.Types", "3.3.9");

        public IntentModule(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; set; }
        public string Version { get; set; }
    }
}
