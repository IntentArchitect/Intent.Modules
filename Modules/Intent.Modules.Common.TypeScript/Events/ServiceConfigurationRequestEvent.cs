using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Events;

public class ServiceConfigurationRequestEvent
{
    public ServiceConfigurationRequestEvent(string importBinding, string moduleSpecifier)
    {
        ImportBinding = importBinding;
        ModuleSpecifier = moduleSpecifier;
    }

    public string ImportBinding { get; set; }

    public string ModuleSpecifier { get; set; }
}
