using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<CSharpTemplateModel> GetCSharpTemplateModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CSharpTemplateModel.SpecializationTypeId)
                .Select(x => new CSharpTemplateModel(x))
                .ToList();
        }

        public static IList<NuGetPackageModel> GetNuGetPackageModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(NuGetPackageModel.SpecializationTypeId)
                .Select(x => new NuGetPackageModel(x))
                .ToList();
        }

        public static IList<NuGetPackagesModel> GetNuGetPackagesModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(NuGetPackagesModel.SpecializationTypeId)
                .Select(x => new NuGetPackagesModel(x))
                .ToList();
        }

    }
}