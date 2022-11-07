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
        public static IList<ApplicationModel> GetApplicationModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ApplicationModel.SpecializationTypeId)
                .Select(x => new ApplicationModel(x))
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