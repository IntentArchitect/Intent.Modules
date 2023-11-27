using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    [IntentManaged(Mode.Merge)]
    public class EventingPackageExtensionModel : EventingPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public EventingPackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public ApplicationModel Application => UnderlyingPackage.ChildElements
            .GetElementsOfType(ApplicationModel.SpecializationTypeId)
            .Select(x => new ApplicationModel(x))
            .SingleOrDefault();

    }
}