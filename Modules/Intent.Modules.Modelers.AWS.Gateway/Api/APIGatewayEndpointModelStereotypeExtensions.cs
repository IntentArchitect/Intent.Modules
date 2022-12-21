using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Gateway.Api
{
    public static class APIGatewayEndpointModelStereotypeExtensions
    {
        public static APIGatewayEndpointSettings GetAPIGatewayEndpointSettings(this APIGatewayEndpointModel model)
        {
            var stereotype = model.GetStereotype("API Gateway Endpoint Settings");
            return stereotype != null ? new APIGatewayEndpointSettings(stereotype) : null;
        }


        public static bool HasAPIGatewayEndpointSettings(this APIGatewayEndpointModel model)
        {
            return model.HasStereotype("API Gateway Endpoint Settings");
        }

        public static bool TryGetAPIGatewayEndpointSettings(this APIGatewayEndpointModel model, out APIGatewayEndpointSettings stereotype)
        {
            if (!HasAPIGatewayEndpointSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new APIGatewayEndpointSettings(model.GetStereotype("API Gateway Endpoint Settings"));
            return true;
        }

        public class APIGatewayEndpointSettings
        {
            private IStereotype _stereotype;

            public APIGatewayEndpointSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public MethodOptions Method()
            {
                return new MethodOptions(_stereotype.GetProperty<string>("Method"));
            }

            public string Path()
            {
                return _stereotype.GetProperty<string>("Path");
            }

            public class MethodOptions
            {
                public readonly string Value;

                public MethodOptions(string value)
                {
                    Value = value;
                }

                public MethodOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "GET":
                            return MethodOptionsEnum.GET;
                        case "POST":
                            return MethodOptionsEnum.POST;
                        case "PUT":
                            return MethodOptionsEnum.PUT;
                        case "PATCH":
                            return MethodOptionsEnum.PATCH;
                        case "HEAD":
                            return MethodOptionsEnum.HEAD;
                        case "DELETE":
                            return MethodOptionsEnum.DELETE;
                        case "OPTIONS":
                            return MethodOptionsEnum.OPTIONS;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
                public bool IsPATCH()
                {
                    return Value == "PATCH";
                }
                public bool IsHEAD()
                {
                    return Value == "HEAD";
                }
                public bool IsDELETE()
                {
                    return Value == "DELETE";
                }
                public bool IsOPTIONS()
                {
                    return Value == "OPTIONS";
                }
            }

            public enum MethodOptionsEnum
            {
                GET,
                POST,
                PUT,
                PATCH,
                HEAD,
                DELETE,
                OPTIONS
            }
        }

    }
}