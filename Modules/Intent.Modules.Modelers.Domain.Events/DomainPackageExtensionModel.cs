using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Merge)]
    public class DomainPackageExtensionModel : DomainPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public DomainPackageExtensionModel(IPackage package) : base(package)
        {
        }

        public IList<DomainEventModel> DomainEvents => UnderlyingPackage.ChildElements
            .GetElementsOfType(DomainEventModel.SpecializationTypeId)
            .Select(x => new DomainEventModel(x))
            .ToList();

    }
}