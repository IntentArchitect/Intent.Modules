using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.CSharp.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<CSharpTemplateModel> GetCSharpTemplateModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CSharpTemplateModel.SpecializationTypeId)
                .Select(x => new CSharpTemplateModel(x))
                .ToList();
        }

    }
}