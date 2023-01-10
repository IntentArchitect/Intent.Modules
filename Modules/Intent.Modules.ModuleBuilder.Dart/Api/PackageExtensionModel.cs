using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Dart.Api
{
    [IntentManaged(Mode.Merge)]
    public class PackageExtensionModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageExtensionModel(IPackage package) : base(package)
        {
        }

        public IList<DartFileTemplateModel> DartTemplates => UnderlyingPackage.ChildElements
            .GetElementsOfType(DartFileTemplateModel.SpecializationTypeId)
            .Select(x => new DartFileTemplateModel(x))
            .ToList();

    }
}