using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class ServiceModelExtensions
    {
        public static HttpServiceSettings GetHttpServiceSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("Http Service Settings");
            return stereotype != null ? new HttpServiceSettings(stereotype) : null;
        }

        public static bool HasHttpServiceSettings(this ServiceModel model)
        {
            return model.HasStereotype("Http Service Settings");
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

    }
}