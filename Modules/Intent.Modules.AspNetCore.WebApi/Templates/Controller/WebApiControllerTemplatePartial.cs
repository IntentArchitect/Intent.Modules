using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.AspNetCore.WebApi.Templates.Controller
{
    partial class WebApiControllerTemplate : CSharpTemplateBase<ServiceModel>, ITemplate, IHasTemplateDependencies, IHasAssemblyDependencies, IHasNugetDependencies, IHasDecorators<WebApiControllerDecoratorBase>, IDeclareUsings, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.AspNetCore.WebApi.Controller";
        private IList<WebApiControllerDecoratorBase> _decorators = new List<WebApiControllerDecoratorBase>();

        public WebApiControllerTemplate(IProject project, ServiceModel model)
            : base(Identifier, project, model)
        {
        }

        public override void OnCreated()
        {
            Types.DefaultCollectionFormat = "List<{0}>";
            Types.AddClassTypeSource(CSharpTypeSource.Create(ExecutionContext, DTOTemplate.IDENTIFIER, "List<{0}>"));
        }

        public IEnumerable<string> DeclareUsings()
        {
            return GetDecorators().SelectMany(x => x.DeclareUsings(Model));
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(ServiceContractTemplate.IDENTIFIER)
            };
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new IAssemblyReference[]
            {
                new GacAssemblyReference("System.Transactions")
            };
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Controller",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public string GetServiceInterfaceName()
        {
            var serviceContractTemplate = ExecutionContext.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.IDENTIFIER, x => x.Id == Model.Id));
            return NormalizeNamespace($"{serviceContractTemplate.Namespace}.{serviceContractTemplate.ClassName}");
        }

        private string GetRoute()
        {
            return Model.GetStereotypeProperty("Http", "Route", "api/[controller]");
        }

        public string DeclarePrivateVariables()
        {
            return GetDecorators().Aggregate(x => x.DeclarePrivateVariables(Model));
        }

        public string ConstructorParams()
        {
            return GetDecorators().Aggregate(x => x.ConstructorParams(Model));
        }

        public string ConstructorInit()
        {
            return GetDecorators().Aggregate(x => x.ConstructorInit(Model));
        }

        public IEnumerable<string> PayloadPropertyDecorators(ParameterModel parameter)
        {
            return GetDecorators().SelectMany(x => x.PayloadPropertyDecorators(parameter));
        }

        public string BeginOperation(OperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeginOperation(Model, operation));
        }

        public string BeforeTransaction(OperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeforeTransaction(Model, operation));
        }

        public string BeforeCallToAppLayer(OperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeforeCallToAppLayer(Model, operation));
        }

        public string AfterCallToAppLayer(OperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.AfterCallToAppLayer(Model, operation));
        }

        public string AfterTransaction(OperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.AfterTransaction(Model, operation));
        }

        public string OnExceptionCaught(OperationModel operation)
        {
            var onExceptionCaught = GetDecorators().Aggregate(x => x.OnExceptionCaught(Model, operation));

            if (GetDecorators().Any(x => x.HandlesCaughtException()))
            {
                return onExceptionCaught;
            }

            return onExceptionCaught + @"
                throw;";
        }

        public string OnDispose()
        {
            return GetDecorators().Aggregate(x => x.OnDispose(Model));
        }

        public string ClassMethods()
        {
            return GetDecorators().Aggregate(x => x.ClassMethods(Model));
        }

        public void AddDecorator(WebApiControllerDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<WebApiControllerDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        private string GetSecurityAttribute(OperationModel o)
        {
            if (o.HasStereotype("Secured") || Model.HasStereotype("Secured"))
            {
                var roles = o.GetStereotypeProperty<string>("Secured", "Roles");
                return string.IsNullOrWhiteSpace(roles)
                    ? "[Authorize]" 
                    : $"[Authorize(Roles = \"{roles}\")]";
            }
            return "[AllowAnonymous]";
        }

        private string GetOperationParameters(OperationModel operation)
        {
            if (!operation.Parameters.Any())
            {
                return string.Empty;
            }
            var verb = GetHttpVerb(operation);
            switch (verb)
            {
                case HttpVerb.POST:
                case HttpVerb.PUT:
                    return operation.Parameters.Select(x => $"{GetParameterBindingAttribute(operation, x)}{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => $"{x}, {y}");
                case HttpVerb.GET:
                case HttpVerb.DELETE:
                    if (operation.Parameters.Any(x => x.TypeReference.Element.SpecializationType == "DTO"))
                    {
                        Logging.Log.Warning($@"Intent.AspNetCore.WebApi: [{Model.Name}.{operation.Name}] Passing objects into HTTP {GetHttpVerb(operation)} operations is not well supported by this module.
    We recommend using a POST or PUT verb");
                        // Log warning
                    }
                    return operation.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
                default:
                    throw new NotSupportedException($"{verb} not supported");
            }
        }

        private string GetOperationCallParameters(OperationModel operation)
        {
            if (!operation.Parameters.Any())
            {
                return string.Empty;
            }

            var verb = GetHttpVerb(operation);
            switch (verb)
            {
                case HttpVerb.POST:
                case HttpVerb.PUT:
                case HttpVerb.GET:
                case HttpVerb.DELETE:
                    return operation.Parameters.Select(x => x.Name).Aggregate((x, y) => $"{x}, {y}");
                default:
                    throw new NotSupportedException($"{verb} not supported");
            }
        }

        private string GetOperationReturnType(OperationModel operation)
        {
            if (operation.TypeReference.Element == null)
            {
                return "void";
            }
            return GetTypeName(operation.TypeReference);
        }

        private HttpVerb GetHttpVerb(OperationModel operation)
        {
            var verb = operation.GetStereotypeProperty("Http", "Verb", "AUTO").ToUpper();
            if (verb != "AUTO")
            {
                return Enum.TryParse(verb, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            }
            if (operation.TypeReference.Element == null || operation.Parameters.Any(x => x.TypeReference.Element.SpecializationType == "DTO"))
            {
                var hasIdParam = operation.Parameters.Any(x => x.Name.ToLower().EndsWith("id") && x.TypeReference.Element.SpecializationType != "DTO");
                if (hasIdParam && new[] {"delete", "remove"}.Any(x => operation.Name.ToLower().Contains(x)))
                {
                    return HttpVerb.DELETE;
                }
                return hasIdParam ? HttpVerb.PUT : HttpVerb.POST;
            }
            return HttpVerb.GET;
        }

        private string GetPath(OperationModel operation)
        {
            var path = operation.GetStereotypeProperty<string>("Http", "Route")?.ToLower();
            return path ?? operation.Name.ToLower();
        }

        private string GetParameterBindingAttribute(OperationModel operation, ParameterModel parameter)
        {
            const string ParameterBinding = "Parameter Binding";
            const string PropertyType = "Type";
            const string PropertyCustomType = "Custom Type";
            const string CustomValue = "Custom";

            if (parameter.HasStereotype(ParameterBinding))
            {
                var attributeName = parameter.GetStereotypeProperty<string>(ParameterBinding, PropertyType);
                if(string.Equals(attributeName, CustomValue, StringComparison.OrdinalIgnoreCase))
                {
                    var customAttributeValue = parameter.GetStereotypeProperty<string>(ParameterBinding, PropertyCustomType);
                    if(string.IsNullOrWhiteSpace(customAttributeValue))
                    {
                        throw new Exception("Parameter Binding was set to custom but no Custom attribute type was specified");
                    }
                    return $"[{customAttributeValue}]";
                }
                return $"[{attributeName}]";
            }

            if (operation.Parameters.Count(p => p.TypeReference.Element.SpecializationType == "DTO") == 1 
                && parameter.TypeReference.Element.SpecializationType == "DTO")
            {
                return "[FromBody]";
            }
            return string.Empty;
        }
    }

    internal enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
