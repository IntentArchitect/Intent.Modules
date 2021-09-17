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
        public static IList<UniqueConstraintModel> GetUniqueConstraintModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(UniqueConstraintModel.SpecializationTypeId)
                .Select(x => new UniqueConstraintModel(x))
                .ToList();
        }

    }
}