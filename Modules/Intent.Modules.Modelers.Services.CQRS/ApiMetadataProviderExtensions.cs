using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<CommandModel> GetCommandModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CommandModel.SpecializationTypeId)
                .Select(x => new CommandModel(x))
                .ToList();
        }

        public static IList<QueryModel> GetQueryModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(QueryModel.SpecializationTypeId)
                .Select(x => new QueryModel(x))
                .ToList();
        }

    }
}