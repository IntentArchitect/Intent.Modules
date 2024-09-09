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
    public static class OperationModelStereotypeExtensions
    {
        public static ApiVersionSettings GetApiVersionSettings(this OperationModel model)
        {
            var stereotype = model.GetStereotype(ApiVersionSettings.DefinitionId);
            return stereotype != null ? new ApiVersionSettings(stereotype) : null;
        }


        public static bool HasApiVersionSettings(this OperationModel model)
        {
            return model.HasStereotype(ApiVersionSettings.DefinitionId);
        }

        public static bool TryGetApiVersionSettings(this OperationModel model, out ApiVersionSettings stereotype)
        {
            if (!HasApiVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApiVersionSettings(model.GetStereotype(ApiVersionSettings.DefinitionId));
            return true;
        }

        public static FileTransfer GetFileTransfer(this OperationModel model)
        {
            var stereotype = model.GetStereotype(FileTransfer.DefinitionId);
            return stereotype != null ? new FileTransfer(stereotype) : null;
        }


        public static bool HasFileTransfer(this OperationModel model)
        {
            return model.HasStereotype(FileTransfer.DefinitionId);
        }

        public static bool TryGetFileTransfer(this OperationModel model, out FileTransfer stereotype)
        {
            if (!HasFileTransfer(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileTransfer(model.GetStereotype(FileTransfer.DefinitionId));
            return true;
        }
        public static HttpSettings GetHttpSettings(this OperationModel model)
        {
            var stereotype = model.GetStereotype(HttpSettings.DefinitionId);
            return stereotype != null ? new HttpSettings(stereotype) : null;
        }

        public static bool HasHttpSettings(this OperationModel model)
        {
            return model.HasStereotype(HttpSettings.DefinitionId);
        }

        public static bool TryGetHttpSettings(this OperationModel model, out HttpSettings stereotype)
        {
            if (!HasHttpSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HttpSettings(model.GetStereotype(HttpSettings.DefinitionId));
            return true;
        }

        public static OpenAPISettings GetOpenAPISettings(this OperationModel model)
        {
            var stereotype = model.GetStereotype(OpenAPISettings.DefinitionId);
            return stereotype != null ? new OpenAPISettings(stereotype) : null;
        }


        public static bool HasOpenAPISettings(this OperationModel model)
        {
            return model.HasStereotype(OpenAPISettings.DefinitionId);
        }

        public static bool TryGetOpenAPISettings(this OperationModel model, out OpenAPISettings stereotype)
        {
            if (!HasOpenAPISettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OpenAPISettings(model.GetStereotype(OpenAPISettings.DefinitionId));
            return true;
        }

        public static Secured GetSecured(this OperationModel model)
        {
            var stereotype = model.GetStereotype(Secured.DefinitionId);
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static bool HasSecured(this OperationModel model)
        {
            return model.HasStereotype(Secured.DefinitionId);
        }

        public static bool TryGetSecured(this OperationModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype(Secured.DefinitionId));
            return true;
        }

        public static Unsecured GetUnsecured(this OperationModel model)
        {
            var stereotype = model.GetStereotype(Unsecured.DefinitionId);
            return stereotype != null ? new Unsecured(stereotype) : null;
        }

        public static bool HasUnsecured(this OperationModel model)
        {
            return model.HasStereotype(Unsecured.DefinitionId);
        }

        public static bool TryGetUnsecured(this OperationModel model, out Unsecured stereotype)
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

        public class FileTransfer
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d30e48e8-389e-4b70-84fd-e3bac44cfe19";

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
            public const string DefinitionId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

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