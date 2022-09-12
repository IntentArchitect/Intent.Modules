using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Repositories.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<RepositoryModel> GetRepositoryModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(RepositoryModel.SpecializationTypeId)
                .Select(x => new RepositoryModel(x))
                .ToList();
        }

    }
}