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
            var stereotype = model.GetStereotype("20855f03-c663-4ec6-b106-de06be98f1fe");
            return stereotype != null ? new ApiVersionSettings(stereotype) : null;
        }


        public static bool HasApiVersionSettings(this ServiceModel model)
        {
            return model.HasStereotype("20855f03-c663-4ec6-b106-de06be98f1fe");
        }

        public static bool TryGetApiVersionSettings(this ServiceModel model, out ApiVersionSettings stereotype)
        {
            if (!HasApiVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApiVersionSettings(model.GetStereotype("20855f03-c663-4ec6-b106-de06be98f1fe"));
            return true;
        }
        public static HttpServiceSettings GetHttpServiceSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("c29224ec-d473-4b95-ad4a-ec55c676c4fd");
            return stereotype != null ? new HttpServiceSettings(stereotype) : null;
        }

        public static bool HasHttpServiceSettings(this ServiceModel model)
        {
            return model.HasStereotype("c29224ec-d473-4b95-ad4a-ec55c676c4fd");
        }

        public static bool TryGetHttpServiceSettings(this ServiceModel model, out HttpServiceSettings stereotype)
        {
            if (!HasHttpServiceSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HttpServiceSettings(model.GetStereotype("c29224ec-d473-4b95-ad4a-ec55c676c4fd"));
            return true;
        }

        public static Secured GetSecured(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("a9eade71-1d56-4be7-a80c-81046c0c978b");
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static bool HasSecured(this ServiceModel model)
        {
            return model.HasStereotype("a9eade71-1d56-4be7-a80c-81046c0c978b");
        }

        public static bool TryGetSecured(this ServiceModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype("a9eade71-1d56-4be7-a80c-81046c0c978b"));
            return true;
        }

        public static Unsecured GetUnsecured(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("8b65c29e-1448-43ac-a92a-0e0f86efd6c6");
            return stereotype != null ? new Unsecured(stereotype) : null;
        }

        public static bool HasUnsecured(this ServiceModel model)
        {
            return model.HasStereotype("8b65c29e-1448-43ac-a92a-0e0f86efd6c6");
        }

        public static bool TryGetUnsecured(this ServiceModel model, out Unsecured stereotype)
        {
            if (!HasUnsecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Unsecured(model.GetStereotype("8b65c29e-1448-43ac-a92a-0e0f86efd6c6"));
            return true;
        }

        public class ApiVersionSettings
        {
            private IStereotype _stereotype;

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

        public class Secured
        {
            private IStereotype _stereotype;

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

        public class Unsecured
        {
            private IStereotype _stereotype;

            public Unsecured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}