using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.ModuleBuilder
{
    public class IntentModule
    {
        public static readonly IntentModule IntentRoslynWeaver = new IntentModule("Intent.OutputManager.RoslynWeaver", "3.0.2");
        public static readonly IntentModule IntentCommon = new IntentModule("Intent.Common", "3.0.6");
        public static readonly IntentModule IntentCommonTypes = new IntentModule("Intent.Common.Types", "3.0.5");

        public IntentModule(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; set; }
        public string Version { get; set; }
    }
}
