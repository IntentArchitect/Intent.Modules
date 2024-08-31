using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        public static IDesigner Domain(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.Domain(application.Id);
        }

        public static IDesigner Domain(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, DomainDesignerId);
        }

    }
}