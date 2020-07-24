using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public static IDesigner Domain(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.GetDesigner(application.Id, "Domain");
        }

    }
}