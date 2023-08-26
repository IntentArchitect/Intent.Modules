using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.DomainInteractions.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<DiagramModel> GetDiagramModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DiagramModel.SpecializationTypeId)
                .Select(x => new DiagramModel(x))
                .ToList();
        }

    }
}