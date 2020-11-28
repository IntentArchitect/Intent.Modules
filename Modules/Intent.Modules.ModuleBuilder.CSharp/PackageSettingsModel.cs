using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp
{
    [IntentManaged(Mode.Merge)]
    public class PackageSettingsModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageSettingsModel(IPackage package) : base(package)
        {
        }

        public IList<CSharpTemplateModel> CSharpTemplates => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == CSharpTemplateModel.SpecializationType)
            .Select(x => new CSharpTemplateModel(x))
            .ToList();

    }
}