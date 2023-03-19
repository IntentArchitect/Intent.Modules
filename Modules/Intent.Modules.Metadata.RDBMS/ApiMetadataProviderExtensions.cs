using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<TriggerModel> GetTriggerModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TriggerModel.SpecializationTypeId)
                .Select(x => new TriggerModel(x))
                .ToList();
        }

    }
}