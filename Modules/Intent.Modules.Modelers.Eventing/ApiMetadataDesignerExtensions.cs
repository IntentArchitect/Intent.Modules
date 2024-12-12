using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modelers.Eventing.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        private static bool _applicationElementCheckPerformed;

        public const string EventingDesignerId = "822e4254-9ced-4dd1-ad56-500b861f7e4d";
        public static IDesigner Eventing(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.Eventing(application.Id);
        }

        public static IDesigner Eventing(this IMetadataManager metadataManager, string applicationId)
        {
            if (!_applicationElementCheckPerformed)
            {
                var applicationElement = metadataManager.Services(applicationId).GetApplicationModels().FirstOrDefault()?.InternalElement;
                if (applicationElement != null)
                {
                    throw new Exception(
                        "\"Application\" element type detected in Services Designer, you will need to migrate from the " +
                        "removed Eventing designer modelling paradigm. Refer to " +
                        "https://docs.intentarchitect.com/articles/application-development/modelling/services-designer/message-based-integration-modeling/message-based-integration-modeling.html#migrating-from-the-eventing-designer " +
                        "for more information.");
                }

                _applicationElementCheckPerformed = true;
            }

            return metadataManager.GetDesigner(applicationId, EventingDesignerId);
        }

    }
}