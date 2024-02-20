using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class CommandModelStereotypeExtensions
    {
        public static ApiVersionSettings GetApiVersionSettings(this CommandModel model)
        {
            var stereotype = model.GetStereotype("Api Version Settings");
            return stereotype != null ? new ApiVersionSettings(stereotype) : null;
        }


        public static bool HasApiVersionSettings(this CommandModel model)
        {
            return model.HasStereotype("Api Version Settings");
        }

        public static bool TryGetApiVersionSettings(this CommandModel model, out ApiVersionSettings stereotype)
        {
            if (!HasApiVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApiVersionSettings(model.GetStereotype("Api Version Settings"));
            return true;
        }

        public static FileTransfer GetFileTransfer(this CommandModel model)
        {
            var stereotype = model.GetStereotype("File Transfer");
            return stereotype != null ? new FileTransfer(stereotype) : null;
        }


        public static bool HasFileTransfer(this CommandModel model)
        {
            return model.HasStereotype("File Transfer");
        }

        public static bool TryGetFileTransfer(this CommandModel model, out FileTransfer stereotype)
        {
            if (!HasFileTransfer(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileTransfer(model.GetStereotype("File Transfer"));
            return true;
        }
        public static HttpSettings GetHttpSettings(this CommandModel model)
        {
            var stereotype = model.GetStereotype("Http Settings");
            return stereotype != null ? new HttpSettings(stereotype) : null;
        }


        public static bool HasHttpSettings(this CommandModel model)
        {
            return model.HasStereotype("Http Settings");
        }

        public static bool TryGetHttpSettings(this CommandModel model, out HttpSettings stereotype)
        {
            if (!HasHttpSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HttpSettings(model.GetStereotype("Http Settings"));
            return true;
        }

        public static OpenAPISettings GetOpenAPISettings(this CommandModel model)
        {
            var stereotype = model.GetStereotype("OpenAPI Settings");
            return stereotype != null ? new OpenAPISettings(stereotype) : null;
        }


        public static bool HasOpenAPISettings(this CommandModel model)
        {
            return model.HasStereotype("OpenAPI Settings");
        }

        public static bool TryGetOpenAPISettings(this CommandModel model, out OpenAPISettings stereotype)
        {
            if (!HasOpenAPISettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OpenAPISettings(model.GetStereotype("OpenAPI Settings"));
            return true;
        }

        public static Secured GetSecured(this CommandModel model)
        {
            var stereotype = model.GetStereotype("Secured");
            return stereotype != null ? new Secured(stereotype) : null;
        }


        public static bool HasSecured(this CommandModel model)
        {
            return model.HasStereotype("Secured");
        }

        public static bool TryGetSecured(this CommandModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype("Secured"));
            return true;
        }

        public static Unsecured GetUnsecured(this CommandModel model)
        {
            var stereotype = model.GetStereotype("Unsecured");
            return stereotype != null ? new Unsecured(stereotype) : null;
        }


        public static bool HasUnsecured(this CommandModel model)
        {
            return model.HasStereotype("Unsecured");
        }

        public static bool TryGetUnsecured(this CommandModel model, out Unsecured stereotype)
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

        public class FileTransfer
        {
            private IStereotype _stereotype;

            public FileTransfer(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string MimeTypeFilter()
            {
                return _stereotype.GetProperty<string>("Mime Type Filter");
            }

            public int? MaximumFileSizeInBytes()
            {
                return _stereotype.GetProperty<int?>("Maximum File Size (in bytes)");
            }

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

            public ReturnTypeMediatypeOptions ReturnTypeMediatype()
            {
                return new ReturnTypeMediatypeOptions(_stereotype.GetProperty<string>("Return Type Mediatype"));
            }

            public class VerbOptions
            {
                public readonly string Value;

                public VerbOptions(string value)
                {
                    Value = value;
                }

                public VerbOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "GET":
                            return VerbOptionsEnum.GET;
                        case "POST":
                            return VerbOptionsEnum.POST;
                        case "PUT":
                            return VerbOptionsEnum.PUT;
                        case "PATCH":
                            return VerbOptionsEnum.PATCH;
                        case "DELETE":
                            return VerbOptionsEnum.DELETE;
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
                public bool IsDELETE()
                {
                    return Value == "DELETE";
                }
            }

            public enum VerbOptionsEnum
            {
                GET,
                POST,
                PUT,
                PATCH,
                DELETE
            }
            public class ReturnTypeMediatypeOptions
            {
                public readonly string Value;

                public ReturnTypeMediatypeOptions(string value)
                {
                    Value = value;
                }

                public ReturnTypeMediatypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return ReturnTypeMediatypeOptionsEnum.Default;
                        case "application/json":
                            return ReturnTypeMediatypeOptionsEnum.ApplicationJson;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsApplicationJson()
                {
                    return Value == "application/json";
                }
            }

            public enum ReturnTypeMediatypeOptionsEnum
            {
                Default,
                ApplicationJson
            }
        }

        public class OpenAPISettings
        {
            private IStereotype _stereotype;

            public OpenAPISettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string OperationId()
            {
                return _stereotype.GetProperty<string>("OperationId");
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