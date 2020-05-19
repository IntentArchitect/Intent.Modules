using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.ModuleBuilder
{
    public class IntentModule
    {
        public static readonly IntentModule IntentRoslynWeaver = new IntentModule("Intent.OutputManager.RoslynWeaver", "2.0.0");
        public static readonly IntentModule IntentCommon = new IntentModule("Intent.Common", "2.0.4");
        public static readonly IntentModule IntentCommonTypes = new IntentModule("Intent.Common.Types", "2.0.1");

        public IntentModule(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; set; }
        public string Version { get; set; }
    }
}
