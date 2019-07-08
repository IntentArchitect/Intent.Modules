using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.ModuleBuilder
{
    public class IntentModule
    {
        public static readonly IntentModule IntentRoslynWeaver = new IntentModule {Name = "Intent.OutputManager.RoslynWeaver", Version = "2.0.0"};
        public static readonly IntentModule IntentDomainModeler = new IntentModule {Name = "Intent.Modelers.Domain", Version = "2.0.0"};
        public static readonly IntentModule IntentServicesModeler = new IntentModule {Name = "Intent.Modelers.Services", Version = "2.0.0"};
        public static readonly IntentModule IntentEventingModeler = new IntentModule {Name = "Intent.Modelers.Eventing", Version = "1.1.0"};
        public static readonly IntentModule IntentCommon = new IntentModule {Name = "Intent.Common", Version = "2.0.0"};
        public static readonly IntentModule IntentCommonTypes = new IntentModule {Name = "Intent.Common.Types", Version = "2.0.0"};

        public string Name { get; set; }
        public string Version { get; set; }
    }
}
