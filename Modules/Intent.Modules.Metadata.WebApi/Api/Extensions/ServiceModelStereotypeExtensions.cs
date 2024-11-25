using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class ServiceModelStereotypeExtensions
    {
        public static ApiVersionSettings GetApiVersionSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype(ApiVersionSettings.DefinitionId);
            return stereotype != null ? new ApiVersionSettings(stereotype) : null;
        }


        public static bool HasApiVersionSettings(this ServiceModel model)
        {
            return model.HasStereotype(ApiVersionSettings.DefinitionId);
        }

        public static bool TryGetApiVersionSettings(this ServiceModel model, out ApiVersionSettings stereotype)
        {
            if (!HasApiVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApiVersionSettings(model.GetStereotype(ApiVersionSettings.DefinitionId));
            return true;
        }
        public static HttpServiceSettings GetHttpServiceSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype(HttpServiceSettings.DefinitionId);
            return stereotype != null ? new HttpServiceSettings(stereotype) : null;
        }

        public static bool HasHttpServiceSettings(this ServiceModel model)
        {
            return model.HasStereotype(HttpServiceSettings.DefinitionId);
        }

        public static bool TryGetHttpServiceSettings(this ServiceModel model, out HttpServiceSettings stereotype)
        {
            if (!HasHttpServiceSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HttpServiceSettings(model.GetStereotype(HttpServiceSettings.DefinitionId));
            return true;
        }

        public static OpenAPISettings GetOpenAPISettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype(OpenAPISettings.DefinitionId);
            return stereotype != null ? new OpenAPISettings(stereotype) : null;
        }


        public static bool HasOpenAPISettings(this ServiceModel model)
        {
            return model.HasStereotype(OpenAPISettings.DefinitionId);
        }

        public static bool TryGetOpenAPISettings(this ServiceModel model, out OpenAPISettings stereotype)
        {
            if (!HasOpenAPISettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OpenAPISettings(model.GetStereotype(OpenAPISettings.DefinitionId));
            return true;
        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public static Secured GetSecured(ServiceModel model)
        {
            var stereotype = model.GetStereotype(Secured.DefinitionId);
            return stereotype != null ? new Secured(stereotype) : null;
        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public static IReadOnlyCollection<Secured> GetSecureds(ServiceModel model)
        {
            var stereotypes = model
                .GetStereotypes(Secured.DefinitionId)
                .Select(stereotype => new Secured(stereotype))
                .ToArray();

            return stereotypes;
        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public static bool HasSecured(ServiceModel model)
        {
            return model.HasStereotype(Secured.DefinitionId);
        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public static bool TryGetSecured(ServiceModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype(Secured.DefinitionId));
            return true;
        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public static Unsecured GetUnsecured(ServiceModel model)
        {
            var stereotype = model.GetStereotype(Unsecured.DefinitionId);
            return stereotype != null ? new Unsecured(stereotype) : null;
        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public static bool HasUnsecured(ServiceModel model)
        {
            return model.HasStereotype(Unsecured.DefinitionId);
        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public static bool TryGetUnsecured(ServiceModel model, out Unsecured stereotype)
        {
            if (!HasUnsecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Unsecured(model.GetStereotype(Unsecured.DefinitionId));
            return true;
        }

        public class ApiVersionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "20855f03-c663-4ec6-b106-de06be98f1fe";

            public ApiVersionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement[] ApplicableVersions()
            {
                return _stereotype.GetProperty<IElement[]>("Applicable Versions") ?? new IElement[0];
            }

        }


        public class HttpServiceSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";

            public HttpServiceSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
            }

        }

        public class OpenAPISettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "b6197544-7e0e-4900-a6e2-9747fb7e4ac4";

            public OpenAPISettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool Ignore()
            {
                return _stereotype.GetProperty<bool>("Ignore");
            }

            public string OperationId()
            {
                return _stereotype.GetProperty<string>("OperationId");
            }

        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public class Secured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "a9eade71-1d56-4be7-a80c-81046c0c978b";

            public Secured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Roles()
            {
                return _stereotype.GetProperty<string>("Roles");
            }

            public string Policy()
            {
                return _stereotype.GetProperty<string>("Policy");
            }

            public IElement[] SecurityRoles()
            {
                return _stereotype.GetProperty<IElement[]>("Security Roles") ?? new IElement[0];
            }

            public IElement[] SecurityPolicies()
            {
                return _stereotype.GetProperty<IElement[]>("Security Policies") ?? new IElement[0];
            }

        }

        // Kept for binary compatibility, stereotypes have been moved to Intent.Metadata.Security
        [IntentIgnore]
        public class Unsecured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8b65c29e-1448-43ac-a92a-0e0f86efd6c6";

            public Unsecured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}