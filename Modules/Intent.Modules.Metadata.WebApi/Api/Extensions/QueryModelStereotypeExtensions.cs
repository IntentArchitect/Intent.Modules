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
    public static class QueryModelStereotypeExtensions
    {
        public static ApiVersionSettings GetApiVersionSettings(this QueryModel model)
        {
            var stereotype = model.GetStereotype("20855f03-c663-4ec6-b106-de06be98f1fe");
            return stereotype != null ? new ApiVersionSettings(stereotype) : null;
        }


        public static bool HasApiVersionSettings(this QueryModel model)
        {
            return model.HasStereotype("20855f03-c663-4ec6-b106-de06be98f1fe");
        }

        public static bool TryGetApiVersionSettings(this QueryModel model, out ApiVersionSettings stereotype)
        {
            if (!HasApiVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApiVersionSettings(model.GetStereotype("20855f03-c663-4ec6-b106-de06be98f1fe"));
            return true;
        }

        public static FileTransfer GetFileTransfer(this QueryModel model)
        {
            var stereotype = model.GetStereotype("d30e48e8-389e-4b70-84fd-e3bac44cfe19");
            return stereotype != null ? new FileTransfer(stereotype) : null;
        }


        public static bool HasFileTransfer(this QueryModel model)
        {
            return model.HasStereotype("d30e48e8-389e-4b70-84fd-e3bac44cfe19");
        }

        public static bool TryGetFileTransfer(this QueryModel model, out FileTransfer stereotype)
        {
            if (!HasFileTransfer(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileTransfer(model.GetStereotype("d30e48e8-389e-4b70-84fd-e3bac44cfe19"));
            return true;
        }
        public static HttpSettings GetHttpSettings(this QueryModel model)
        {
            var stereotype = model.GetStereotype("b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6");
            return stereotype != null ? new HttpSettings(stereotype) : null;
        }


        public static bool HasHttpSettings(this QueryModel model)
        {
            return model.HasStereotype("b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6");
        }

        public static bool TryGetHttpSettings(this QueryModel model, out HttpSettings stereotype)
        {
            if (!HasHttpSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HttpSettings(model.GetStereotype("b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6"));
            return true;
        }

        public static OpenAPISettings GetOpenAPISettings(this QueryModel model)
        {
            var stereotype = model.GetStereotype("b6197544-7e0e-4900-a6e2-9747fb7e4ac4");
            return stereotype != null ? new OpenAPISettings(stereotype) : null;
        }


        public static bool HasOpenAPISettings(this QueryModel model)
        {
            return model.HasStereotype("b6197544-7e0e-4900-a6e2-9747fb7e4ac4");
        }

        public static bool TryGetOpenAPISettings(this QueryModel model, out OpenAPISettings stereotype)
        {
            if (!HasOpenAPISettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OpenAPISettings(model.GetStereotype("b6197544-7e0e-4900-a6e2-9747fb7e4ac4"));
            return true;
        }

        public static Secured GetSecured(this QueryModel model)
        {
            var stereotype = model.GetStereotype("a9eade71-1d56-4be7-a80c-81046c0c978b");
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static bool HasSecured(this QueryModel model)
        {
            return model.HasStereotype("a9eade71-1d56-4be7-a80c-81046c0c978b");
        }

        public static bool TryGetSecured(this QueryModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype("a9eade71-1d56-4be7-a80c-81046c0c978b"));
            return true;
        }

        public static Unsecured GetUnsecured(this QueryModel model)
        {
            var stereotype = model.GetStereotype("8b65c29e-1448-43ac-a92a-0e0f86efd6c6");
            return stereotype != null ? new Unsecured(stereotype) : null;
        }


        public static bool HasUnsecured(this QueryModel model)
        {
            return model.HasStereotype("8b65c29e-1448-43ac-a92a-0e0f86efd6c6");
        }

        public static bool TryGetUnsecured(this QueryModel model, out Unsecured stereotype)
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

            public SuccessResponseCodeOptions SuccessResponseCode()
            {
                return new SuccessResponseCodeOptions(_stereotype.GetProperty<string>("Success Response Code"));
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
            public class SuccessResponseCodeOptions
            {
                public readonly string Value;

                public SuccessResponseCodeOptions(string value)
                {
                    Value = value;
                }

                public SuccessResponseCodeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "200 (Ok)":
                            return SuccessResponseCodeOptionsEnum._200Ok;
                        case "201 (Created)":
                            return SuccessResponseCodeOptionsEnum._201Created;
                        case "202 (Accepted)":
                            return SuccessResponseCodeOptionsEnum._202Accepted;
                        case "203 (Non-Authoritative Information)":
                            return SuccessResponseCodeOptionsEnum._203NonAuthoritativeInformation;
                        case "204 (No Content)":
                            return SuccessResponseCodeOptionsEnum._204NoContent;
                        case "205 (Reset Content)":
                            return SuccessResponseCodeOptionsEnum._205ResetContent;
                        case "206 (Partial Content)":
                            return SuccessResponseCodeOptionsEnum._206PartialContent;
                        case "207 (Multi-Status)":
                            return SuccessResponseCodeOptionsEnum._207MultiStatus;
                        case "208 (Already Reported)":
                            return SuccessResponseCodeOptionsEnum._208AlreadyReported;
                        case "226 (IM Used)":
                            return SuccessResponseCodeOptionsEnum._226IMUsed;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool Is200Ok()
                {
                    return Value == "200 (Ok)";
                }
                public bool Is201Created()
                {
                    return Value == "201 (Created)";
                }
                public bool Is202Accepted()
                {
                    return Value == "202 (Accepted)";
                }
                public bool Is203NonAuthoritativeInformation()
                {
                    return Value == "203 (Non-Authoritative Information)";
                }
                public bool Is204NoContent()
                {
                    return Value == "204 (No Content)";
                }
                public bool Is205ResetContent()
                {
                    return Value == "205 (Reset Content)";
                }
                public bool Is206PartialContent()
                {
                    return Value == "206 (Partial Content)";
                }
                public bool Is207MultiStatus()
                {
                    return Value == "207 (Multi-Status)";
                }
                public bool Is208AlreadyReported()
                {
                    return Value == "208 (Already Reported)";
                }
                public bool Is226IMUsed()
                {
                    return Value == "226 (IM Used)";
                }
            }

            public enum SuccessResponseCodeOptionsEnum
            {
                _200Ok,
                _201Created,
                _202Accepted,
                _203NonAuthoritativeInformation,
                _204NoContent,
                _205ResetContent,
                _206PartialContent,
                _207MultiStatus,
                _208AlreadyReported,
                _226IMUsed
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