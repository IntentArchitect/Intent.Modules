using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modelers.Services.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<DTOModel> GetDTOModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DTOModel.SpecializationTypeId)
                .Select(x => new DTOModel(x))
                .ToList();
        }

        public static IList<ServiceModel> GetServiceModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ServiceModel.SpecializationTypeId)
                .Select(x => new ServiceModel(x))
                .ToList();
        }

    }
}