using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Services.Api
{
    [IntentManaged(Mode.Merge)]
    public class DomainPackageExtensionModel : DomainPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public DomainPackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<DomainServiceModel> DomainServices => UnderlyingPackage.ChildElements
            .GetElementsOfType(DomainServiceModel.SpecializationTypeId)
            .Select(x => new DomainServiceModel(x))
            .ToList();

    }
}