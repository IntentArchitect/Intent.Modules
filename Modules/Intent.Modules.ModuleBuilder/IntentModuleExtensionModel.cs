using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder
{
    [IntentManaged(Mode.Merge)]
    public class IntentModuleExtensionModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public IntentModuleExtensionModel(IPackage package) : base(package)
        {
        }

        public DesignersFolderModel DesignersFolder => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == DesignersFolderModel.SpecializationType)
            .Select(x => new DesignersFolderModel(x))
            .SingleOrDefault();

    }
}