using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.ModuleBuilder.TypeScript.Api
{
    [IntentManaged(Mode.Merge)]
    public class PackageExtensionModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<TypescriptFileTemplateModel> TypescriptTemplates => UnderlyingPackage.ChildElements
            .GetElementsOfType(TypescriptFileTemplateModel.SpecializationTypeId)
            .Select(x => new TypescriptFileTemplateModel(x))
            .ToList();

    }
}