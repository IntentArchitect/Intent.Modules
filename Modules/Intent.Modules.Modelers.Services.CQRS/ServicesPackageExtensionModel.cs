using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServicesPackageExtensionModel : ServicesPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServicesPackageExtensionModel(IPackage package) : base(package)
        {
        }

        public IList<CQRSServiceModel> CQRSServices => UnderlyingPackage.ChildElements
            .GetElementsOfType(CQRSServiceModel.SpecializationTypeId)
            .Select(x => new CQRSServiceModel(x))
            .ToList();

    }
}