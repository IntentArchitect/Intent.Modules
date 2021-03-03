using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    [IntentManaged(Mode.Merge)]
    public class PackageSettingsModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageSettingsModel(IPackage package) : base(package)
        {
        }

        public IList<CSharpTemplateModel> CSharpTemplates => UnderlyingPackage.ChildElements
            .GetElementsOfType(CSharpTemplateModel.SpecializationTypeId)
            .Select(x => new CSharpTemplateModel(x))
            .ToList();

    }
}