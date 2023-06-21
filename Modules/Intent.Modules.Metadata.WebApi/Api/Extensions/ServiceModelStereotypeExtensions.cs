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
            var stereotype = model.GetStereotype("Api Version Settings");
            return stereotype != null ? new ApiVersionSettings(stereotype) : null;
        }


        public static bool HasApiVersionSettings(this ServiceModel model)
        {
            return model.HasStereotype("Api Version Settings");
        }

        public static bool TryGetApiVersionSettings(this ServiceModel model, out ApiVersionSettings stereotype)
        {
            if (!HasApiVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApiVersionSettings(model.GetStereotype("Api Version Settings"));
            return true;
        }
        public static HttpServiceSettings GetHttpServiceSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("Http Service Settings");
            return stereotype != null ? new HttpServiceSettings(stereotype) : null;
        }

        public static bool HasHttpServiceSettings(this ServiceModel model)
        {
            return model.HasStereotype("Http Service Settings");
        }

        public static bool TryGetHttpServiceSettings(this ServiceModel model, out HttpServiceSettings stereotype)
        {
            if (!HasHttpServiceSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HttpServiceSettings(model.GetStereotype("Http Service Settings"));
            return true;
        }

        public static Secured GetSecured(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("Secured");
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static bool HasSecured(this ServiceModel model)
        {
            return model.HasStereotype("Secured");
        }

        public static bool TryGetSecured(this ServiceModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype("Secured"));
            return true;
        }

        public static Unsecured GetUnsecured(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("Unsecured");
            return stereotype != null ? new Unsecured(stereotype) : null;
        }

        public static bool HasUnsecured(this ServiceModel model)
        {
            return model.HasStereotype("Unsecured");
        }

        public static bool TryGetUnsecured(this ServiceModel model, out Unsecured stereotype)
        {
            if (!HasUnsecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Unsecured(model.GetStereotype("Unsecured"));
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