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
        public static IList<EventingDTOModel> GetEventingDTOModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(EventingDTOModel.SpecializationTypeId)
                .Select(x => new EventingDTOModel(x))
                .ToList();
        }
        public static IList<IntegrationEventHandlerModel> GetIntegrationEventHandlerModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(IntegrationEventHandlerModel.SpecializationTypeId)
                .Select(x => new IntegrationEventHandlerModel(x))
                .ToList();
        }

        public static IList<MessageModel> GetMessageModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(MessageModel.SpecializationTypeId)
                .Select(x => new MessageModel(x))
                .ToList();
        }

    }
}