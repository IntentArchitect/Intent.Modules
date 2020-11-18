using System;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class ModuleDependencyRequiredEvent
    {
        public ModuleDependencyRequiredEvent(string moduleId, string moduleVersion)
        {
            ModuleId = moduleId ?? throw new ArgumentNullException(nameof(moduleId));
            ModuleVersion = moduleVersion ?? throw new ArgumentNullException(nameof(moduleVersion));
        }
        public string ModuleId { get; }
        public string ModuleVersion { get; }
    }
}