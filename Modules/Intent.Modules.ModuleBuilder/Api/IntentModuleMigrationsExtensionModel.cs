using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class IntentModuleMigrationsExtensionModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public IntentModuleMigrationsExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public ModuleMigrationsModel Migrations => UnderlyingPackage.ChildElements
            .GetElementsOfType(ModuleMigrationsModel.SpecializationTypeId)
            .Select(x => new ModuleMigrationsModel(x))
            .SingleOrDefault();

    }
}