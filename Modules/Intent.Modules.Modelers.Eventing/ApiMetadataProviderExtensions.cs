using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ConsumerModel> GetConsumerModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ConsumerModel.SpecializationTypeId)
                .Select(x => new ConsumerModel(x))
                .ToList();
        }

        public static IList<MessageModel> GetMessageModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(MessageModel.SpecializationTypeId)
                .Select(x => new MessageModel(x))
                .ToList();
        }

        public static IList<ProducerModel> GetProducerModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ProducerModel.SpecializationTypeId)
                .Select(x => new ProducerModel(x))
                .ToList();
        }

    }
}