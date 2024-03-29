using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Metadata.WebApi.Api
{
    [Obsolete]
    public static class OperationModelExtensions
    {
        [Obsolete]
        public static HttpSettings GetHttpSettings(OperationModel model)
        {
            var stereotype = model.GetStereotype("Http Settings");
            return stereotype != null ? new HttpSettings(stereotype) : null;
        }

        [Obsolete]
        public static bool HasHttpSettings(OperationModel model)
        {
            return model.HasStereotype("Http Settings");
        }

        [Obsolete]
        public static Secured GetSecured(OperationModel model)
        {
            var stereotype = model.GetStereotype("Secured");
            return stereotype != null ? new Secured(stereotype) : null;
        }

        [Obsolete]
        public static bool HasSecured(OperationModel model)
        {
            return model.HasStereotype("Secured");
        }

        [Obsolete]
        public static Unsecured GetUnsecured(OperationModel model)
        {
            var stereotype = model.GetStereotype("Unsecured");
            return stereotype != null ? new Unsecured(stereotype) : null;
        }

        [Obsolete]
        public static bool HasUnsecured(OperationModel model)
        {
            return model.HasStereotype("Unsecured");
        }


        [Obsolete]
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

        [Obsolete]
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

        [Obsolete]
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