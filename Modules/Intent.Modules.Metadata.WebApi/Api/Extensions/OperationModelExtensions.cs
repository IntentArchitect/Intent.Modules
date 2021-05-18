using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class OperationModelExtensions
    {
        public static HttpSettings GetHttpSettings(this OperationModel model)
        {
            var stereotype = model.GetStereotype("Http Settings");
            return stereotype != null ? new HttpSettings(stereotype) : null;
        }

        public static bool HasHttpSettings(this OperationModel model)
        {
            return model.HasStereotype("Http Settings");
        }

        public static Secured GetSecured(this OperationModel model)
        {
            var stereotype = model.GetStereotype("Secured");
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static bool HasSecured(this OperationModel model)
        {
            return model.HasStereotype("Secured");
        }

        public static Unsecured GetUnsecured(this OperationModel model)
        {
            var stereotype = model.GetStereotype("Unsecured");
            return stereotype != null ? new Unsecured(stereotype) : null;
        }

        public static bool HasUnsecured(this OperationModel model)
        {
            return model.HasStereotype("Unsecured");
        }


        public class HttpSettings
        {
            private IStereotype _stereotype;

            public HttpSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public VerbOptions Verb()
            {
                return new VerbOptions(_stereotype.GetProperty<string>("Verb"));
            }

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
            }

            public class VerbOptions
            {
                public readonly string Value;

                public VerbOptions(string value)
                {
                    Value = value;
                }

                public bool IsGET()
                {
                    return Value == "GET";
                }
                public bool IsPOST()
                {
                    return Value == "POST";
                }
                public bool IsPUT()
                {
                    return Value == "PUT";
                }
                public bool IsDELETE()
                {
                    return Value == "DELETE";
                }
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