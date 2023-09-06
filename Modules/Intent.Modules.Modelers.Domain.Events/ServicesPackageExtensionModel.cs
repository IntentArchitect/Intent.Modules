using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServicesPackageExtensionModel : ServicesPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServicesPackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<DomainEventHandlerModel> DomainEventHandlers => UnderlyingPackage.ChildElements
            .GetElementsOfType(DomainEventHandlerModel.SpecializationTypeId)
            .Select(x => new DomainEventHandlerModel(x))
            .ToList();

    }
}