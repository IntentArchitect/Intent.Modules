using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Services.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<DomainServiceModel> GetDomainServiceModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DomainServiceModel.SpecializationTypeId)
                .Select(x => new DomainServiceModel(x))
                .ToList();
        }

    }
}