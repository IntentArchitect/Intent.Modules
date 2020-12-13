using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Java.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Java
{
    [IntentManaged(Mode.Merge)]
    public class IntentModuleExtensionModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public IntentModuleExtensionModel(IPackage package) : base(package)
        {
        }

        public IList<JavaFileTemplateModel> JavaFiles => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == JavaFileTemplateModel.SpecializationType)
            .Select(x => new JavaFileTemplateModel(x))
            .ToList();

    }
}