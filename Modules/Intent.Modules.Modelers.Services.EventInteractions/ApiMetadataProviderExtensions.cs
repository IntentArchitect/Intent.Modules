using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<IntegrationCommandHandlerModel> GetIntegrationCommandHandlerModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(IntegrationCommandHandlerModel.SpecializationTypeId)
                .Select(x => new IntegrationCommandHandlerModel(x))
                .ToList();
        }
        public static IList<IntegrationEventHandlerModel> GetIntegrationEventHandlerModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(IntegrationEventHandlerModel.SpecializationTypeId)
                .Select(x => new IntegrationEventHandlerModel(x))
                .ToList();
        }

    }
}