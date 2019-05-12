using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.AspNet.WebApi.Templates.Controller
{
    partial class WebApiControllerTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, IPostTemplateCreation, ITemplate, IHasTemplateDependencies, IHasAssemblyDependencies, IHasDecorators<WebApiControllerDecoratorBase>, IDeclareUsings, IBeforeTemplateExecutionHook
    {
        public const string Identifier = "Intent.AspNet.WebApi.Controller";

        private readonly IList<WebApiControllerDecoratorBase> _decorators = new List<WebApiControllerDecoratorBase>();

        public WebApiControllerTemplate(IProject project, IServiceModel model, string identifier = Identifier)
            : base(identifier, project, model)
        {
        }

        public void Created()
        {
            Types.AddClassTypeSource(CSharpTypeSource.InProject(Project, DTOTemplate.IDENTIFIER, "List<{0}>"));
        }

        public IEnumerable<string> DeclareUsings()
        {
            return GetDecorators().SelectMany(x => x.DeclareUsings());
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

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftAspNetWebApi,
                NugetPackages.MicrosoftAspNetWebApiClient,
                NugetPackages.MicrosoftAspNetWebApiCore,
                NugetPackages.MicrosoftAspNetWebApiWebHost,
                NugetPackages.NewtonsoftJson,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: Model.Name + "Controller",
                fileExtension: "cs",
                defaultLocationInProject: "Controllers",
                className: "${Model.Name}Controller",
                @namespace: "${Project.Name}"
                );
        }

        public void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { ContainerRegistrationEvent.InterfaceTypeKey, null},
                { ContainerRegistrationEvent.ConcreteTypeKey, $"{Namespace}.{ClassName}" },
                { ContainerRegistrationEvent.InterfaceTypeTemplateIdKey, null },
                { ContainerRegistrationEvent.ConcreteTypeTemplateIdKey, Identifier },
                { ContainerRegistrationEvent.LifetimeKey, ContainerRegistrationEvent.PerServiceCallLifetime }
            });
        }

        public string GetServiceInterfaceName()
        {
            var serviceContractTemplate = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.IDENTIFIER, x => x.Id == Model.Id));
            return NormalizeNamespace($"{serviceContractTemplate.Namespace}.{serviceContractTemplate.ClassName}");
        }

        private string GetTypeName(ITypeReference typeInfo)
        {
            //var result = NormalizeNamespace(typeInfo.GetQualifiedName(this));
            //if (typeInfo.IsCollection)
            //{
            //    result = "List<" + result + ">";
            //}
            return Types.Get(typeInfo, "List<{0}>");
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

        public IEnumerable<string> PayloadPropertyDecorators(IOperationParameter parameter)
        {
            return GetDecorators().SelectMany(x => x.PayloadPropertyDecorators(parameter));
        }

        public string BeginOperation(IOperation operation)
        {
            return GetDecorators().Aggregate(x => x.BeginOperation(Model, operation));
        }

        public string BeforeTransaction(IOperation operation)
        {
            return GetDecorators().Aggregate(x => x.BeforeTransaction(Model, operation));
        }

        public string BeforeCallToAppLayer(IOperation operation)
        {
            return GetDecorators().Aggregate(x => x.BeforeCallToAppLayer(Model, operation));
        }

        public string AfterCallToAppLayer(IOperation operation)
        {
            return GetDecorators().Aggregate(x => x.AfterCallToAppLayer(Model, operation));
        }

        public string AfterTransaction(IOperation operation)
        {
            return GetDecorators().Aggregate(x => x.AfterTransaction(Model, operation));
        }

        public string OnExceptionCaught(IOperation operation)
        {
            var onExceptionCaught = GetDecorators().Aggregate(x => x.OnExceptionCaught(Model, operation));

            if (GetDecorators().Any(x => x.HandlesCaughtException()))
            {
                return onExceptionCaught;
            }

            return @"
                return InternalServerError(e);";
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

        private string GetSecurityAttribute(IOperation o)
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

        private string GetRoute(IOperation operation)
        {
            return operation.Name.ToLower();
        }

        private static bool RequiresPayloadObject(IOperation operation)
        {
            if (!operation.Parameters.Any())
            {
                return false;
            }

            if (operation.GetStereotypeProperty("Http", "Payload", "AUTO") == "Payload Object")
            {
                return true;
            }

            var verb = GetHttpVerb(operation);
            if (verb != HttpVerb.POST && verb != HttpVerb.PUT)
            {
                return false;
            }

            return operation.Parameters.Count(IsFromBody) > 1;
        }

        private static string GetPayloadObjectTypeName(IOperation operation)
        {
            return $"{operation.Name.ToPascalCase()}BodyPayload";
        }

        private string GetOperationParameters(IOperation operation)
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
                    return RequiresPayloadObject(operation)
                        ? operation.Parameters
                            .Where(x => !IsFromBody(x))
                            .Select(x => $"{GetParameterBindingAttribute(x)}{GetTypeName(x.Type)} {x.Name}")
                            .Concat(new [] { $"{GetPayloadObjectTypeName(operation)} bodyPayload" })
                            .Aggregate((x, y) => $"{x}, {y}")
                        : operation.Parameters
                            .Select(x => $"{GetParameterBindingAttribute(x)}{GetTypeName(x.Type)} {x.Name}")
                            .Aggregate((x, y) => $"{x}, {y}");
                case HttpVerb.GET:
                case HttpVerb.DELETE:
                    if (operation.Parameters.Any(x => GetParameterBindingAttribute(x) == "[FromBody]"))
                    {
                        throw new Exception($"Intent.AspNet.WebApi: [{Model.Name}.{operation.Name}] HTTP {verb} does not support parameters with a [FromBody] attribute.");
                    }

                    if (operation.Parameters.Any(x => x.Type.SpecializationType == "DTO"))
                    {
                        Logging.Log.Warning($@"Intent.AspNet.WebApi: [{Model.Name}.{operation.Name}] Passing complex types into HTTP {verb} operations is not well supported by this module.
    We recommend using a POST or PUT verb.");
                        // Log warning
                    }

                    return operation.Parameters
                        .Select(x => $"{GetTypeName(x.Type)} {x.Name}")
                        .Aggregate((x, y) => x + ", " + y);
                default:
                    throw new NotSupportedException($"{verb} not supported");
            }
        }

        private static string GetOperationCallParameters(IOperation operation)
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
                    return operation.Parameters
                        .Select(x => RequiresPayloadObject(operation) ? $"bodyPayload.{x.Name}" : x.Name)
                        .Aggregate((x, y) => $"{x}, {y}");
                default:
                    throw new NotSupportedException($"{verb} not supported");
            }
        }

        private string GetOperationReturnType(IOperation operation)
        {
            if (operation.ReturnType == null)
            {
                return "void";
            }
            return GetTypeName(operation.ReturnType.Type);
        }

        private static HttpVerb GetHttpVerb(IOperation operation)
        {
            var verb = operation.GetStereotypeProperty("Http", "Verb", "AUTO").ToUpper();
            if (verb != "AUTO")
            {
                return Enum.TryParse(verb, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            }

            if (operation.ReturnType == null || operation.Parameters.Any(IsFromBody))
            {
                return HttpVerb.POST;
            }

            return HttpVerb.GET;
        }

        private static string GetParameterBindingAttribute(IHasStereotypes stereotypeable)
        {
            const string parameterBinding = "Parameter Binding";
            const string propertyType = "Type";
            const string propertyCustomType = "Custom Type";
            const string customValue = "Custom";

            if (!stereotypeable.HasStereotype(parameterBinding))
            {
                return string.Empty;
            }

            var attributeName = stereotypeable.GetStereotypeProperty<string>(parameterBinding, propertyType);
            if (!string.Equals(attributeName, customValue, StringComparison.OrdinalIgnoreCase))
            {
                return $"[{attributeName}]";
            }

            var customAttributeValue = stereotypeable.GetStereotypeProperty<string>(parameterBinding, propertyCustomType);
            if (string.IsNullOrWhiteSpace(customAttributeValue))
            {
                throw new Exception("Parameter Binding was set to custom but no Custom attribute type was specified");
            }

            return $"[{customAttributeValue}]";
        }

        private static bool IsFromBody(IOperationParameter parameter)
        {
            var csharpPrimitives = new[]
            {
                "guid",
                "System.Guid",

                "bool",
                "Boolean",
                "System.Boolean",
                "byte",
                "Byte",
                "System.Byte",
                "sbyte",
                "SByte",
                "System.SByte",
                "char",
                "Char",
                "System.Char",
                "decimal",
                "Decimal",
                "System.Decimal",
                "double",
                "Double",
                "System.Double",
                "float",
                "Single",
                "System.Single",
                "int",
                "Int32",
                "System.Int32",
                "uint",
                "UInt32",
                "System.UInt32",
                "long",
                "Int64",
                "System.Int64",
                "ulong",
                "UInt64",
                "System.UInt64",
                "short",
                "Int16",
                "System.Int16",
                "ushort",
                "UInt16",
                "System.UInt16",
                "string",
                "String",
                "System.String"
            };

            // NB: Order of conditional checks is important here
            return GetParameterBindingAttribute(parameter) == "[FromBody]" || !csharpPrimitives.Contains(parameter.Type.Name);
        }

        private enum HttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
