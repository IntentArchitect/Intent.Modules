using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modelers.ServiceProxies.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public const string ServiceProxiesDesignerId = "2799aa83-e256-46fe-9589-b96f7d6b09f7";
        public static IDesigner ServiceProxies(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.ServiceProxies(application.Id);
        }

        public static IDesigner ServiceProxies(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, ServiceProxiesDesignerId);
        }

    }
}