using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.TypeScript.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.TypeScript
{
    [IntentManaged(Mode.Merge)]
    public class PackageExtensionModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageExtensionModel(IPackage package) : base(package)
        {
        }

        public IList<TypescriptFileTemplateModel> TypescriptTemplates => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == TypescriptFileTemplateModel.SpecializationType)
            .Select(x => new TypescriptFileTemplateModel(x))
            .ToList();

    }
}